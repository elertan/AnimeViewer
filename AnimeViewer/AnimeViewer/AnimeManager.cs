using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
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

        public CancellationTokenSource CachingUpdateTaskCancellationSource { get; set; }

        /// <summary>
        ///     This event gets called when we are done caching animes
        /// </summary>
        public event EventHandler FinishedCachingAnimes;

        /// <summary>
        ///     Occurs when the cache is removed
        /// </summary>
        public event EventHandler CacheRemoved;

        public event EventHandler<UpdatedCachedAnimeEventArgs> UpdatedCachedAnimeInformation;

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
                DbConnection = await Database.GetConnectionAsync();

            // If it's our first time caching animes, set the last cached page to 1
            if (!Application.Current.Properties.ContainsKey(AnimeManagerLastUpdatedListPagePropertyKey))
                Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 1;
            // Save the settings
            await Application.Current.SavePropertiesAsync();

            // Get our api instance from our losely coupled container (abstract api, so its easily changed to another api if needed)
            var api = ((App) Application.Current).DiContainer.Resolve<IAnimeApi>();
            // Instance is now set and ready to be used
            Instance = new AnimeManager {Api = api};

            // Create settings for our api, so it can store its settings
            var settings = new Dictionary<string, object>();
            // Get all created settings from our total app settings, settings are saved using the full typename, so if we use another api the old settings wont be overridden
            foreach (
                var setting in Application.Current.Properties.Where(prop => prop.Key.Contains(api.GetType().FullName)))
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
        }

        /// <summary>
        ///     Updates and caches animes by the api
        /// </summary>
        /// <returns></returns>
        private async Task UpdateCachedAnimesByApi(CancellationToken token)
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
                if ((animes.Count > 0) && !token.IsCancellationRequested)
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
                    // Update popular references
//                    var popularAnimes = await GetMostPopularAnimesAsync();

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
            // Cancel current updating task if not null
            CachingUpdateTaskCancellationSource?.Cancel();
            // Create new cancel source
            CachingUpdateTaskCancellationSource = new CancellationTokenSource();
            // Get all animes from the database
            var animes = await DbConnection.Table<Anime>().ToListAsync();
#pragma warning disable 4014
            // Start caching all animes on the background
            UpdateCachedAnimesByApi(CachingUpdateTaskCancellationSource.Token)
                .ContinueWith(UpdateCachedAnimesByApiThrowedException, CachingUpdateTaskCancellationSource.Token,
                    TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
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
            if (anime.Episodes == null)
                anime.Episodes = await DbConnection.GetAllWithChildrenAsync<Episode>(ep => ep.AnimeId == anime.Id);

            anime.LastVisitedDateTime = DateTime.Now;
            // If it already contains all information, simply return that anime
            if (anime.ContainsAllInformation)
            {
                await UpdateAnimeInformationForCachedAnime(anime);
                return anime;
            }

            // Retrieve the anime with full information from the api
            var a = await Api.GetFullAnimeInformationByPageUrlAsync(anime.PageUrl);
            // Assign same id to overwrite current cached anime

            anime.Summary = a.Summary;
            anime.Episodes = a.Episodes;
            foreach (var episode in anime.Episodes)
                episode.Anime = anime;
            anime.Genres = a.Genres;
            //anime.ImageUrl = a.ImageUrl;
            anime.Language = a.Language;

            // Set it to contain all information
            anime.ContainsAllInformation = true;
            // Update database entries (with children to also affect the episodes and their children)
            await DbConnection.InsertAllWithChildrenAsync(anime.Episodes);
            await UpdateAnimeInformationForCachedAnime(anime);
            return anime;
        }

        /// <summary>
        ///     Update database information for the given anime
        /// </summary>
        /// <param name="anime">Anime to update</param>
        /// <returns></returns>
        public async Task UpdateAnimeInformationForCachedAnime(Anime anime)
        {
            //await DbConnection.UpdateWithChildrenAsync(anime);
            await DbConnection.UpdateAsync(anime);
            await DbConnection.UpdateAllAsync(anime.Episodes);

            //OnUpdatedCachedAnimeInformation(new UpdatedCachedAnimeEventArgs {Anime = anime});
        }

        /// <summary>
        ///     Removes all cached animes and their episodes
        /// </summary>
        /// <returns></returns>
        public async Task RemoveCache()
        {
            // Cancel updating task if possible
            CachingUpdateTaskCancellationSource?.Cancel();

            await DbConnection.DeleteAllAsync<Anime>();
            await DbConnection.DeleteAllAsync<Episode>();
            Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 1;
            await Application.Current.SavePropertiesAsync();

            OnCacheRemoved();
        }

        /// <summary>
        ///     Gets the video sources (urls) for the given episode by quality (1080p, 720p, 480p, 360p)
        /// </summary>
        /// <param name="episode">The episode to get the videosources for</param>
        /// <returns></returns>
        public async Task<IEnumerable<VideoSource>> GetVideoSourcesByEpisode(Episode episode)
        {
            // Retrieve the sources via our api with the episodes url
            var sources = await Api.GetVideoSourcesByEpisodeUrlAsync(episode.EpisodeUrl);
            if (Device.OS != TargetPlatform.iOS) return sources;

            // iOS requires some extra fixing if the sources arent of googlevideo yet
            var httpClient = new HttpClient();
            foreach (var source in sources.Where(s => !s.SourceUrl.Contains("googlevideo")))
            {
                var response = await httpClient.GetAsync(source.SourceUrl, HttpCompletionOption.ResponseHeadersRead);
                source.SourceUrl = response.RequestMessage.RequestUri.ToString();
            }
            return sources;
        }

        public async Task<IEnumerable<Anime>> GetMostPopularAnimesAsync()
        {
            var animes = await Api.GetMostPopularAnimesAsync();
            var dbAnimes = await DbConnection.Table<Anime>().ToListAsync();
            await DbConnection.UpdateAllAsync(dbAnimes.Select(anime =>
            {
                var popularAnime = animes.FirstOrDefault(a => (a.Name == anime.Name) && (a.Language == anime.Language));
                anime.IsHot = popularAnime != null;
                return anime;
            }));
            return dbAnimes.Where(anime => anime.IsHot);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Anime>> GetUpdatedAnimes()
        {
            var animes = await Api.GetUpdatedAnimesAsync();
            return animes;
        }

        protected virtual void OnCacheRemoved()
        {
            CacheRemoved?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUpdatedCachedAnimeInformation(UpdatedCachedAnimeEventArgs e)
        {
            UpdatedCachedAnimeInformation?.Invoke(this, e);
        }
    }
}