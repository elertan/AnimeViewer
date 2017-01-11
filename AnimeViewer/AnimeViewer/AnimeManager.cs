using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using AnimeViewer.Services;
using Microsoft.Practices.Unity;
using SQLite.Net.Async;
using SQLiteNetExtensionsAsync.Extensions;
using Xamarin.Forms;

namespace AnimeViewer
{
    public class AnimeManager
    {
        /// <summary>
        ///     The key to store our last cached anime page
        /// </summary>
        private static readonly string AnimeManagerLastUpdatedListPagePropertyKey = "AnimeManagerLastUpdatedListPage";

        /// <summary>
        ///     Database Connection
        /// </summary>
        public static SQLiteAsyncConnection DbConnection { get; set; }

        /// <summary>
        ///     Instance of ourself (Singleton pattern)
        /// </summary>
        public static AnimeManager Instance { get; set; }

        /// <summary>
        ///     The api
        /// </summary>
        public IAnimeApi Api { get; set; }

        /// <summary>
        ///     This event gets called when we are done caching animes
        /// </summary>
        public event EventHandler FinishedCachingAnimes;

        /// <summary>
        ///     This method is used to invoke our FinishedCachingAnimes event
        /// </summary>
        private void OnFinishedCachingAnimes()
        {
            FinishedCachingAnimes?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     This gets called each time we got new data from our api, specifically new animes that we cached
        /// </summary>
        public event EventHandler<AnimesUpdatedEventArgs> CachedAnimesUpdated;

        /// <summary>
        ///     This gets called when there is an error connection with the api
        /// </summary>
        public event EventHandler AnimeManagerApiConnectionError;

        /// <summary>
        ///     This method invokes the CachedAnimesUpdated event
        /// </summary>
        /// <param name="addedAnimes">The new added animes</param>
        /// <param name="page">The corrosponding page</param>
        private void OnCachedAnimesUpdated(List<Anime> addedAnimes, int page)
        {
            CachedAnimesUpdated?.Invoke(this, new AnimesUpdatedEventArgs {Animes = addedAnimes, Page = page});
        }

        /// <summary>
        ///     This method invokes the AnimeManagerApiConnectionError event
        /// </summary>
        private void OnAnimeManagerApiConnectionError()
        {
            AnimeManagerApiConnectionError?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Initializes the AnimeManager instance for use
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAsync()
        {
            // If we dont have a connection yet, get one
            if (DbConnection == null)
                DbConnection = await Database.GetConnection();

            // If it's our first time caching animes, set the last cached page to 1
            if (!Application.Current.Properties.ContainsKey(AnimeManagerLastUpdatedListPagePropertyKey))
                Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 1;
            // Save the settings
            await Application.Current.SavePropertiesAsync();

            // Get our api instance from our losely coupled container (abstract api, so its easily changed to another api if needed)
            var api = ((App) Application.Current).DiContainer.Resolve<IAnimeApi>();
            // Create settings for our api, so it can store its settings
            var settings = new Dictionary<string, object>();
            // Get all created settings from our total app settings, settings are saved using the full typename, so if we use another api the old settings wont be overridden
            foreach (var setting in Application.Current.Properties.Where(prop => prop.Key.Contains(api.GetType().FullName)))
            {
                // Get the key that was stored using the api's code
                var key = setting.Key.Replace(api.GetType().FullName, "");
                // Put the settings in our settings variable again
                settings.Add(key, setting.Value);
            }

            // Add them back on the api
            api.Settings = settings;
            // Initialize the api
            await api.Initialize();
            // Once we have initialized our api, we save our settings back
            foreach (var setting in api.Settings)
                Application.Current.Properties[api.GetType().FullName + setting.Key] = setting.Value;
            await Application.Current.SavePropertiesAsync();

            // Instance is now set and ready to be used
            Instance = new AnimeManager {Api = api};
        }

        /// <summary>
        ///     Updates and caches animes by the api
        /// </summary>
        /// <returns></returns>
        private async Task UpdateCachedAnimesByApi()
        {
            // Infinite loop, gets broken if there are no more animes to be found on a page
            while (true)
            {
                // Retrieve our last cached page
                var currentListPage = (int) Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey];
                // Use our api to get the animes by the corresponding list page
                var result = await Api.GetAnimesByListPageNumberAsync(currentListPage);
                // Store it into a list type variable
                var animes = new List<Anime>(result);
                // Did we retrieve more than 1 anime?
                if (animes.Count > 0)
                {
                    // Store it all into our database
                    await DbConnection.InsertOrIgnoreAllAsync(animes);
                    // Add 1 to last cached page, so next time we cache the next page
                    Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = currentListPage + 1;
                    await Application.Current.SavePropertiesAsync();
                    // Call our event so subcribed methods can use our newly cached animes
                    OnCachedAnimesUpdated(animes, currentListPage);
                }
                else
                {
                    // We are done caching
                    OnFinishedCachingAnimes();
                    // Leave the loop and end the caching process
                    break;
                }
            }
        }

        // This gets called if an error occured during the caching process
        private void UpdateCachedAnimesByApiThrowedException(Task task)
        {
            // Retrieve the exception thrown via the task
            var ex = task.Exception;
            // Call our event
            OnAnimeManagerApiConnectionError();
        }

        /// <summary>
        ///     Returns the full list of currently cached animes
        /// </summary>
        /// <returns></returns>
        public async Task<List<Anime>> GetAnimeListAsync()
        {
            // Get all animes from the database
            var animes = await DbConnection.Table<Anime>().ToListAsync();
#pragma warning disable 4014
            // Start caching all animes on the background
            UpdateCachedAnimesByApi().ContinueWith(UpdateCachedAnimesByApiThrowedException, TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
            return animes;
        }

        /// <summary>
        ///     Get full information of the anime given
        /// </summary>
        /// <param name="anime">The anime that requires more information</param>
        /// <returns>The anime with more information</returns>
        public async Task<Anime> GetFullAnimeInformation(Anime anime)
        {
            // Get the anime from the database that corresponds to the given anime (so we can store the full data after using its id)
            var animes = await DbConnection.GetAllWithChildrenAsync<Anime>(dto => dto.Name == anime.Name);
            var a = animes.First();
            // If it already contains all information, simply return that anime
            if (a.ContainsAllInformation)
                return a;

            // Retrieve the anime with full information from the api
            a = await Api.GetFullAnimeInformationByPageUrlAsync(anime.PageUrl);
            // Assign same id to overwrite current cached anime
            a.Id = anime.Id;

            // TEMPORARY SINCE PAGEURL IS NULL
            a.PageUrl = anime.PageUrl;
            // Set it to contain all information
            a.ContainsAllInformation = true;
            // Update database entries (with children to also affect the episodes and their children)
            await DbConnection.InsertAllWithChildrenAsync(a.Episodes);
            await UpdateAnimeInformationForCachedAnime(a);
            return a;
        }

        /// <summary>
        ///     Update database information for the given anime
        /// </summary>
        /// <param name="anime">Anime to update</param>
        /// <returns></returns>
        public async Task UpdateAnimeInformationForCachedAnime(Anime anime)
        {
            await DbConnection.UpdateWithChildrenAsync(anime);
            //await DbConnection.UpdateAsync(anime);
            //await DbConnection.UpdateAllAsync(anime.Episodes);
        }

        /// <summary>
        ///     TODO: FINISH IMPLEMENTATION NOT WORKING: Removes the current cached animes
        /// </summary>
        /// <returns></returns>
        public async Task RemoveCache()
        {
            throw new NotSupportedException("Not yet finished");

            await DbConnection.DeleteAllAsync<Anime>();
            await DbConnection.DeleteAllAsync<Episode>();
            Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 1;
            await Application.Current.SavePropertiesAsync();
        }

        /// <summary>
        ///     Gets the video sources (urls) for the given episode by quality (1080p, 720p, 480p, 360p)
        /// </summary>
        /// <param name="episode">The episode to get the videosources for</param>
        /// <returns></returns>
        public async Task<IEnumerable<VideoSource>> GetVideoSourcesByEpisode(Episode episode)
        {
            // Retrieve the sources via our api with the episodes url
            return await Api.GetVideoSourcesByEpisodeUrlAsync(episode.EpisodeUrl);
        }
    }
}