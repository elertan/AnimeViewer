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
    /// <summary>
    ///     This is the implementation of the anime api for KissAnime(.ru)
    /// </summary>
    public class KissanimeApi : IAnimeApi
    {
        /// <summary>
        ///     The host address
        /// </summary>
        public static readonly string HostAddress = "http://kissanime.ru";

        /// <summary>
        ///     The innerhandler, used to read and write cookies to
        /// </summary>
        private readonly HttpClientHandler _handler;

        public KissanimeApi()
        {
            // Create a new handler
            _handler = new HttpClientHandler();
            // Create a new clearancehandler (to bypass cloudflare protection) and use our handler as innerhandler, and set the base address so we can use relative url's.
            HttpClient = new HttpClient(new ClearanceHandler(_handler)) {BaseAddress = new Uri(HostAddress)};
        }

        /// <summary>
        ///     Has the api initialized
        /// </summary>
        public bool HasInitialized { get; set; }

        /// <summary>
        ///     The used httpclient
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        ///     The settings
        /// </summary>
        public Dictionary<string, object> Settings { get; set; }

        public async Task<IEnumerable<Anime>> GetAnimesByListPageNumberAsync(int pageNumber)
        {
            // Wait until the api has initialized, check each 100ms
            while (!HasInitialized) await Task.Delay(100);
            // Get the pages html for the page
            var responseString = await HttpClient.GetStringAsync($"/AnimeList?page={pageNumber}");
            // Scrape the html for the animes
            return ExtractFromListingTable(responseString);
        }

        public async Task<Anime> GetFullAnimeInformationByPageUrlAsync(string pageUrl)
        {
            // Wait until the api has initialized, check each 100ms
            while (!HasInitialized) await Task.Delay(100);
            // Get the pages html for the page
            var responseString = await HttpClient.GetStringAsync(pageUrl);

            // Scrape the html
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
            foreach (var episodeNode in tableListing.Descendants("a"))
            {
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
            // Wait until the api has initialized, check each 100ms
            while (!HasInitialized) await Task.Delay(100);
            // Get the html response
            var responseString = await HttpClient.GetStringAsync(episodeUrl);

            // scrape the html
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var sources = new List<VideoSource>();

            var selElem = htmlDoc.DocumentNode.Descendants("select").First(sel => sel.Id == "selectQuality");
            foreach (var option in selElem.Descendants("option"))
            {
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
            // TODO: FIX THIS COOKIES INVALID AFTER LONG PERIOD INACTIVE
            //foreach (var setting in Settings)
            //    _handler.CookieContainer.Add(uri, new Cookie(setting.Key, (string) setting.Value));

            // Retrieve homepage to bypass cloudflare and gain the cookies, 3 retries
            var response = await HttpClient.GetAsync("/");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Initialization of the KissAnime Api failed.");
            //catch (AggregateException ex) when (ex.InnerException is CloudFlareClearanceException)
            //{
            //    // After all retries, clearance still failed.
            //    var e = ex.InnerException;
            //}
            //catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
            //{
            //    // Looks like we ran into a timeout. Too many clearance attempts?
            //    // Maybe you should increase client.Timeout as each attempt will take about five seconds.
            //    var e = ex.InnerException;
            //}

            // Set imageservice httpclient so it can download the images (it needs the cloudflare cookies)
            ImageService.Instance.Initialize(new Configuration {HttpClient = HttpClient});

            // Save cookies
            foreach (var cookie in _handler.CookieContainer.GetCookies(uri))
            {
                var c = (Cookie) cookie;
                Settings[c.Name] = c.Value;
            }
            HasInitialized = true;
        }

        /// <summary>
        ///     Logic for extracting the animes from the listing table
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private IEnumerable<Anime> ExtractFromListingTable(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            try
            {
                var tableListing =
                    htmlDoc.DocumentNode.Descendants("table")
                        .FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));

                var animeTableRows = tableListing.Descendants("tr").Where(tr => tr.Descendants("td").Count() != 0);

                var animes = new List<Anime>();

                foreach (var animeTableRow in animeTableRows)
                {
                    var anime = new Anime();

                    anime.Name = animeTableRow.Descendants("td").First().Descendants("a").First().InnerText.Trim();
                    var tags = animeTableRow.Descendants("td").First().Descendants("img");
                    foreach (var tag in tags)
                    {
                        if (!tag.Attributes.Contains("title")) continue;
                        // Anime tags for updated and hot
                        if (tag.Attributes["title"].Value.Contains("updated")) anime.JustUpdated = true;
                        if (tag.Attributes["title"].Value.Contains("Popular")) anime.IsHot = true;
                    }

                    var doc = new HtmlDocument();
                    doc.LoadHtml(animeTableRow.Descendants("td").First().Attributes["title"].Value);
                    anime.ImageUrl = doc.DocumentNode.Descendants("img").First().Attributes["src"].Value;
                    anime.PageUrl = HostAddress + doc.DocumentNode.Descendants("div").First().Descendants("a").First().Attributes["href"].Value;

                    animes.Add(anime);
                }
                return animes;
            }
            catch
            {
            }

            return new List<Anime>();
        }
    }
}