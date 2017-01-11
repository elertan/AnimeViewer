using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AnimeViewer.Models;

namespace AnimeViewer.Services
{
    /// <summary>
    ///     Handles all api interaction
    /// </summary>
    public interface IAnimeApi
    {
        /// <summary>
        ///     The httpclient the api uses to connect via http
        /// </summary>
        HttpClient HttpClient { get; set; }

        /// <summary>
        ///     The settings that the api can read and write to
        /// </summary>
        Dictionary<string, object> Settings { get; set; }

        /// <summary>
        ///     Initializes the api for use
        /// </summary>
        /// <returns></returns>
        Task Initialize();

        /// <summary>
        ///     Gets animes by a list page number
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <returns></returns>
        Task<IEnumerable<Anime>> GetAnimesByListPageNumberAsync(int pageNumber);

        /// <summary>
        ///     Gets all anime information by the anime's page url
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        Task<Anime> GetFullAnimeInformationByPageUrlAsync(string pageUrl);

        /// <summary>
        ///     Gets all videosources by the url of an episode
        /// </summary>
        /// <param name="episodeUrl">The episode url</param>
        /// <returns></returns>
        Task<IEnumerable<VideoSource>> GetVideoSourcesByEpisodeUrlAsync(string episodeUrl);
    }
}