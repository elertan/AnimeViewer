using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudFlareUtilities;
using HtmlAgilityPack;
using KissAnime.EventArguments;
using KissAnime.Models;

namespace KissAnime
{
    public class Api
    {
        public static readonly string HostAddress = "http://kissanime.to";
        private readonly CookieContainer _cookies;
        private readonly HttpClientHandler _handler;
        public readonly HttpClient HttpClient;

        public Api()
        {
            _handler = new HttpClientHandler {CookieContainer = _cookies};
            HttpClient = new HttpClient(new ClearanceHandler(_handler)) {BaseAddress = new Uri(HostAddress)};
        }

        public static bool HasInitialized { get; private set; }

        public static Api Instance { get; private set; }

        public static event EventHandler<ApiInitializedEventArgs> Initialized;

        private static void OnInitialized(Api api)
        {
            Initialized?.Invoke(api, new ApiInitializedEventArgs {CookieContainer = api._handler.CookieContainer});
        }

        /// <summary>
        ///     Initializes the api for usage (clears javascript challenge from cloudflare)
        /// </summary>
        public static async Task Initialize()
        {
            await Initialize(new Dictionary<string, string>(), () => { });
        }

        public static async Task Initialize(Dictionary<string, string> cookies, Action preBroadcastAction)
        {
            var api = new Api();
            foreach (var cookie in cookies)
                api._handler.CookieContainer.Add(new Uri(HostAddress), new Cookie(cookie.Key, cookie.Value));
            var response = await api.HttpClient.GetAsync("/");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Initialization of the KissAnime Api failed.");
            Instance = api;
            HasInitialized = true;
            preBroadcastAction();
            OnInitialized(api);
        }

        public static async Task Initialize(Action preBroadcastAction)
        {
            await Initialize(new Dictionary<string, string>(), preBroadcastAction);
        }

        private List<Anime> ExtractFromListingTable(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tableListing =
                htmlDoc.DocumentNode.Descendants("table")
                    .FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));

            var animeTableRows = tableListing.Descendants("tr").Where(tr => tr.Descendants("td").Count() != 0);

            var animes = new List<Anime>();
            foreach (var animeTableRow in animeTableRows)
                try
                {
                    var anime = new Anime();

                    anime.Name = animeTableRow.Descendants("td").First().Descendants("a").First().InnerText.Trim();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(animeTableRow.Descendants("td").First().Attributes["title"].Value);
                    anime.ImageUri = new Uri(doc.DocumentNode.Descendants("img").First().Attributes["src"].Value,
                        UriKind.Absolute);
                    anime.AnimeUri =
                        new Uri(
                            doc.DocumentNode.Descendants("div").First().Descendants("a").First().Attributes["href"]
                                .Value, UriKind.Relative);

                    animes.Add(anime);
                }
                catch
                {
                }

            return animes;
        }

        public async Task<List<Anime>> GetAnimesByKeyword(string keyword)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("keyword", keyword)
            });
            var response = await HttpClient.PostAsync("/Search/Anime", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var tableListing =
                htmlDoc.DocumentNode.Descendants("table")
                    .FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));
            if (tableListing == null)
            {
                await Task.Delay(500);
                return await GetAnimesByKeyword(keyword);
            }

            return ExtractFromListingTable(responseString);
        }

        public async Task<List<Anime>> GetAnimesByListPage(int page)
        {
            var responseString = await HttpClient.GetStringAsync($"/AnimeList?page={page}");
            return ExtractFromListingTable(responseString);
        }

        public async Task<List<Anime>> GetFeaturedAnimes()
        {
            var responseString = await HttpClient.GetStringAsync("/AnimeList/NewAndHot");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var tableListing =
                htmlDoc.DocumentNode.Descendants("table")
                    .FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));
            if (tableListing == null)
            {
                await Task.Delay(500);
                return await GetFeaturedAnimes();
            }

            return ExtractFromListingTable(responseString);
        }

        public async Task<List<Anime>> GetNewestAnimes()
        {
            var responseString = await HttpClient.GetStringAsync("/AnimeList/Newest");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var tableListing =
                htmlDoc.DocumentNode.Descendants("table")
                    .FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));
            if (tableListing == null)
            {
                await Task.Delay(500);
                return await GetFeaturedAnimes();
            }
            var animeTableRows = tableListing.Descendants("tr").Where(tr => tr.Descendants("td").Count() != 0);

            var animes = new List<Anime>();

            foreach (var animeTableRow in animeTableRows)
                try
                {
                    var anime = new Anime();

                    anime.Name = animeTableRow.Descendants("td").First().Descendants("a").First().InnerText.Trim();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(animeTableRow.Descendants("td").First().Attributes["title"].Value);
                    anime.ImageUri = new Uri(doc.DocumentNode.Descendants("img").First().Attributes["src"].Value,
                        UriKind.Absolute);
                    anime.AnimeUri =
                        new Uri(
                            doc.DocumentNode.Descendants("div").First().Descendants("a").First().Attributes["href"]
                                .Value, UriKind.Relative);

                    animes.Add(anime);
                }
                catch
                {
                }

            return animes;
        }

        public async Task<List<Anime>> GetPopularAnimes()
        {
            var responseString = await HttpClient.GetStringAsync("/AnimeList/MostPopular");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var tableListing =
                htmlDoc.DocumentNode.Descendants("table")
                    .FirstOrDefault(n => n.Attributes.Contains("class") && (n.Attributes["class"].Value == "listing"));
            if (tableListing == null)
            {
                await Task.Delay(500);
                return await GetFeaturedAnimes();
            }
            var animeTableRows = tableListing.Descendants("tr").Where(tr => tr.Descendants("td").Count() != 0);

            var animes = new List<Anime>();

            foreach (var animeTableRow in animeTableRows)
                try
                {
                    var anime = new Anime();

                    anime.Name = animeTableRow.Descendants("td").First().Descendants("a").First().InnerText.Trim();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(animeTableRow.Descendants("td").First().Attributes["title"].Value);
                    anime.ImageUri = new Uri(doc.DocumentNode.Descendants("img").First().Attributes["src"].Value,
                        UriKind.Absolute);
                    anime.AnimeUri =
                        new Uri(
                            doc.DocumentNode.Descendants("div").First().Descendants("a").First().Attributes["href"]
                                .Value, UriKind.Relative);

                    animes.Add(anime);
                }
                catch
                {
                }

            return animes;
        }

        public async Task<Anime> GetAnimeByUri(Uri animeUri)
        {
            var responseString = await HttpClient.GetStringAsync(animeUri);

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

            anime.Episodes = new List<Episode>();
            foreach (var episodeNode in tableListing.Descendants("a"))
            {
                var episode = new Episode();

                episode.Anime = anime;
                episode.Name = episodeNode.InnerText.Trim().Replace(anime.Name, "");
                episode.EpisodeUri = new Uri(episodeNode.Attributes["href"].Value, UriKind.Relative);

                anime.Episodes.Add(episode);
            }
            anime.Genres = new List<string>();
            foreach (
                var genreNode in
                htmlDoc.DocumentNode.Descendants("a")
                    .Where(n => n.Attributes.Contains("href") && n.Attributes["href"].Value.Contains("/Genre/")))
                anime.Genres.Add(genreNode.InnerText);


            return anime;
        }

        public async Task<Anime> GetFullAnime(Anime anime)
        {
            var totalAnime = await GetAnimeByUri(anime.AnimeUri);
            anime.Name = totalAnime.Name;
            anime.Summary = totalAnime.Summary;
            anime.Episodes = totalAnime.Episodes;
            anime.Genres = totalAnime.Genres;

            // repoint owner to the current anime
            foreach (var episode in anime.Episodes) episode.Anime = anime;
            return anime;
        }

        public async Task<List<VideoSource>> GetVideoSourcesByEpisodeUri(Uri episodeUri)
        {
            var responseString = await HttpClient.GetStringAsync(episodeUri);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var sources = new List<VideoSource>();

            var selElem = htmlDoc.DocumentNode.Descendants("select").First(sel => sel.Id == "selectQuality");
            foreach (var option in selElem.Descendants("option"))
            {
                var source = new VideoSource();

                var data = Convert.FromBase64String(option.Attributes["value"].Value);
                source.SourceUri = new Uri(Encoding.UTF8.GetString(data, 0, data.Length));
                source.Quality = option.NextSibling.InnerText;

                sources.Add(source);
            }

            return sources;
        }

        public async Task<Stream> GetImageStreamByUri(Uri imageUri)
        {
            var response = await HttpClient.GetAsync(imageUri);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}