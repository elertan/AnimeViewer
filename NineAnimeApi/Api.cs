﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NineAnimeApi.Models;

namespace NineAnimeApi
{
    public class Api
    {
        public static readonly string Hostname = "http://9anime.to/";
        public static readonly Uri HostnameUri = new Uri(Hostname, UriKind.Absolute);
        public static readonly HttpClient HttpClient = new HttpClient {BaseAddress = HostnameUri};

        private List<Anime> ExtractAnimesFromPage(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var animeNodes = htmlDoc.DocumentNode.Descendants("div").Where(div => div.Attributes.Contains("class") && (div.Attributes["class"].Value == "item"));
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

        private async Task<List<Anime>> ExtractAnimesFromPageAsync(string html)
        {
            return await Task.Run(() => ExtractAnimesFromPage(html));
        }

        public async Task<List<Anime>> GetAnimesAscendingOrderByPageAsync(int pageNumber)
        {
            var response = await HttpClient.GetStringAsync($"filter?sort=title%3Aasc&page={pageNumber}");
            return await ExtractAnimesFromPageAsync(response);
        }
    }
}