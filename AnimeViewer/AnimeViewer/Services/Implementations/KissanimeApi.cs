using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AnimeViewer.Models;
using CloudFlareUtilities;
using FFImageLoading;
using FFImageLoading.Config;
using HtmlAgilityPack;

namespace AnimeViewer.Services.Implementations
{
	public class KissanimeApi : IAnimeApi
	{
		public static readonly string HostAddress = "http://kissanime.ru";
		private readonly HttpClientHandler _handler;

		public KissanimeApi()
		{
			_handler = new HttpClientHandler();
			HttpClient = new HttpClient(new ClearanceHandler(_handler)) { BaseAddress = new Uri(HostAddress) };
		}

		public bool HasInitialized { get; set; }

		public HttpClient HttpClient { get; set; }
		public Dictionary<string, object> Settings { get; set; }

		public async Task<IEnumerable<Anime>> GetAnimesByListPageNumberAsync(int pageNumber)
		{
			if (!HasInitialized) await Initialize();
			var responseString = await HttpClient.GetStringAsync($"/AnimeList?page={pageNumber}");
			return ExtractFromListingTable(responseString);
		}

		public async Task<Anime> GetFullAnimeInformationByPageUrlAsync(string pageUrl)
		{
			if (!HasInitialized) await Initialize();
			var responseString = await HttpClient.GetStringAsync(pageUrl);

			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(responseString);

			var anime = new Anime();
			var tableListing =
				htmlDoc.DocumentNode.Descendants("table")
					.FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));

			anime.Name =
				htmlDoc.DocumentNode.Descendants("a")
					.First(a => a.Attributes.Contains("Class") && (a.Attributes["Class"].Value == "bigChar"))
					.InnerText;
			anime.Summary =
				htmlDoc.DocumentNode.Descendants("span")
					.First(
						span =>
							span.Attributes.Contains("class") && (span.Attributes["class"].Value == "info") &&
							(span.InnerText == "Summary:"))
					.NextSibling.NextSibling.InnerText.Trim();
			anime.ImageUrl = htmlDoc.DocumentNode.Descendants("img").First(img => img.Attributes.Contains("height") && (img.Attributes["height"].Value == "250px")).Attributes["src"].Value;

			var episodes = new List<Episode>();
			foreach (var episodeNode in tableListing.Descendants("a")) {
				var episode = new Episode();

				episode.Anime = anime;
				//episode.AnimeId = anime.Id;
				episode.Name = episodeNode.InnerText.Trim().Replace(anime.Name, "");
				episode.EpisodeUrl = episodeNode.Attributes["href"].Value;

				episodes.Add(episode);
			}
			anime.Episodes = episodes;

			var genres = "";
			foreach (
				var genreNode in
				htmlDoc.DocumentNode.Descendants("a")
					.Where(n => n.Attributes.Contains("href") && n.Attributes["href"].Value.Contains("/Genre/")))
				genres += genreNode.InnerText + ",";
			if (genres.Length > 0) genres = genres.Remove(genres.Length - 1, 1);
			anime.Genres = genres;

			return anime;
		}

		public async Task<IEnumerable<VideoSource>> GetVideoSourcesByEpisodeUrlAsync(string episodeUrl)
		{
			var responseString = await HttpClient.GetStringAsync(episodeUrl);
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(responseString);

			var sources = new List<VideoSource>();

			var selElem = htmlDoc.DocumentNode.Descendants("select").First(sel => sel.Id == "selectQuality");
			foreach (var option in selElem.Descendants("option")) {
				var source = new VideoSource();

				var data = Convert.FromBase64String(option.Attributes["value"].Value);
				source.SourceUrl = Encoding.UTF8.GetString(data, 0, data.Length);
				source.Quality = option.NextSibling.InnerText;

				sources.Add(source);
			}

			return sources;
		}

		public async Task Initialize()
		{
			// Add cookies to request data
			var uri = new Uri(HostAddress);
			foreach (var setting in Settings)
				_handler.CookieContainer.Add(uri, new Cookie(setting.Key, (string)setting.Value));

			var response = await HttpClient.GetAsync("/");
			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException("Initialization of the KissAnime Api failed.");
			ImageService.Instance.Initialize(new Configuration { HttpClient = HttpClient });

			// Save cookies
			foreach (var cookie in _handler.CookieContainer.GetCookies(uri)) {
				var c = (Cookie)cookie;
				Settings[c.Name] = c.Value;
			}
			HasInitialized = true;
		}

		private IEnumerable<Anime> ExtractFromListingTable(string html)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(html);
			try {
				var tableListing =
					htmlDoc.DocumentNode.Descendants("table")
						.FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));

				var animeTableRows = tableListing.Descendants("tr").Where(tr => tr.Descendants("td").Count() != 0);

				var animes = new List<Anime>();

				foreach (var animeTableRow in animeTableRows) {
					var anime = new Anime();

					anime.Name = animeTableRow.Descendants("td").First().Descendants("a").First().InnerText.Trim();

					var doc = new HtmlDocument();
					doc.LoadHtml(animeTableRow.Descendants("td").First().Attributes["title"].Value);
					anime.ImageUrl = doc.DocumentNode.Descendants("img").First().Attributes["src"].Value;
					anime.PageUrl = HostAddress + doc.DocumentNode.Descendants("div").First().Descendants("a").First().Attributes["href"].Value;

					animes.Add(anime);
				}
				return animes;
			} catch {
			}

			return new List<Anime>();
		}
	}
}