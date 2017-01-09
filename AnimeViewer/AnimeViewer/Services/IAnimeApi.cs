using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AnimeViewer.Models;

namespace AnimeViewer.Services
{
    public interface IAnimeApi
    {
        HttpClient HttpClient { get; set; }
        Dictionary<string, object> Settings { get; set; }
        Task Initialize();
        Task<IEnumerable<Anime>> GetAnimesByListPageNumberAsync(int pageNumber);
        Task<Anime> GetFullAnimeInformationByPageUrlAsync(string pageUrl);
        Task<IEnumerable<VideoSource>> GetVideoSourcesByEpisodeUrlAsync(string episodeUrl);
    }
}