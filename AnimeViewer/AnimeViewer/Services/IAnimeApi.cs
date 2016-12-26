﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AnimeViewer.Models;

namespace AnimeViewer.Services
{
    public interface IAnimeApi
    {
        HttpClient HttpClient { get; set; }
        Task<IEnumerable<Anime>> GetAnimesByListPageNumberAsync(int pageNumber);
        Task<Anime> GetFullAnimeInformationByPageUrlAsync(string pageUrl);
        Task<IEnumerable<VideoSource>> GetVideoSourcesByEpisodeUrlAsync(string episodeUrl);
    }
}