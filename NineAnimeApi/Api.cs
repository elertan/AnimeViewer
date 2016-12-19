﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NineAnimeApi.Models;

// ReSharper disable PossibleMultipleEnumeration

namespace NineAnimeApi
{
    public class Api
    {
        public static readonly string Hostname = "http://9anime.to/";
        public static readonly Uri HostnameUri = new Uri(Hostname, UriKind.Absolute);
        public static readonly HttpClient HttpClient = new HttpClient {BaseAddress = HostnameUri};

        public int MaxHttpClientRetries { get; set; } = 5;

        public int HttpClientRetryDelay { get; set; } = 1000;

        private List<Anime> ExtractAnimesFromPage(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var animeNodes =
                htmlDoc.DocumentNode.Descendants("div")
                    .Where(div => div.Attributes.Contains("class") && (div.Attributes["class"].Value == "item"));
            var animes = new List<Anime>();
            foreach (var node in animeNodes)
            {
                var lastLinkNode = node.Descendants("a").Last();
                var anime = new Anime
                {
                    PageUrl = lastLinkNode.Attributes["href"].Value,
                    PosterImageUrl = WebUtility.HtmlDecode(node.Descendants("img").First().Attributes["src"].Value),
                    Name = lastLinkNode.InnerText,
                    Type = node.Descendants("div").First().InnerText
                };
                animes.Add(anime);
            }
            return animes;
        }

        private Anime ExtractAnimeFromPage(string html)
        {
            var anime = new Anime();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var itemProps = htmlDoc.DocumentNode.Descendants().Where(div => div.Attributes.Contains("itemprop"));
            anime.Name = itemProps.First(div => div.Attributes["itemprop"].Value == "name").InnerText;

            anime.PosterImageUrl =
                itemProps.First(div => div.Attributes["itemprop"].Value == "image").Attributes["src"].Value;

            anime.Summary = itemProps.First(div => div.Attributes["itemprop"].Value == "description").InnerText;
            anime.AmountOfRatings = int.Parse(itemProps.First(div => div.Attributes["itemprop"].Value == "ratingCount").InnerText);
            anime.Identifier = htmlDoc.DocumentNode.Descendants("div").First(div => div.Attributes.Contains("id") && div.Attributes["id"].Value == "movie").Attributes["data-id"].Value;
            anime.PageUrl = "http://9anime.to/watch/" + anime.Identifier;
            return anime;
        }

        private async Task<List<Anime>> ExtractAnimesFromPageAsync(string html)
        {
            return await Task.Run(() => ExtractAnimesFromPage(html));
        }

        private async Task<Anime> ExtractAnimeFromPageAsync(string html)
        {
            return await Task.Run(() => ExtractAnimeFromPage(html));
        }

        public async Task<List<Anime>> GetAnimesAscendingOrderByPageAsync(int pageNumber)
        {
            // 3 Retries
            for (var i = 0; i < MaxHttpClientRetries; i++)
                try
                {
                    var response = await HttpClient.GetStringAsync($"filter?sort=title%3Aasc&page={pageNumber}");
                    await Task.Delay(1000);
                    return await ExtractAnimesFromPageAsync(response);
                }
                catch
                {
                    await Task.Delay(HttpClientRetryDelay);
                }
            throw new HttpRequestException(nameof(GetAnimesAscendingOrderByPageAsync) + " failed to retrieve animes.");
        }

        public async Task<Anime> GetAnimeByPageUrl(string pageUrl)
        {
            for (var i = 0; i < MaxHttpClientRetries; i++)
                try
                {
                    var response = await HttpClient.GetStringAsync(pageUrl);
                    await Task.Delay(1000);
                    return await ExtractAnimeFromPageAsync(response);
                }
                catch
                {
                    await Task.Delay(HttpClientRetryDelay);
                }
            throw new HttpRequestException(nameof(GetAnimeByPageUrl) + " failed to retrieve anime information.");
        }
    }
}