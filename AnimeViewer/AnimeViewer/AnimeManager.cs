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
        private static readonly string AnimeManagerLastUpdatedListPagePropertyKey = "AnimeManagerLastUpdatedListPage";
        public static SQLiteAsyncConnection DbConnection { get; set; }
        public static AnimeManager Instance { get; set; }

        public IAnimeApi Api { get; set; }

        public event EventHandler FinishedCachingAnimes;

        private void OnFinishedCachingAnimes()
        {
            FinishedCachingAnimes?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<AnimesUpdatedEventArgs> CachedAnimesUpdated;
        public event EventHandler AnimeManagerApiConnectionError;

        private void OnCachedAnimesUpdated(List<Anime> addedAnimes, int page)
        {
            CachedAnimesUpdated?.Invoke(this, new AnimesUpdatedEventArgs {Animes = addedAnimes, Page = page});
        }

        private void OnAnimeManagerApiConnectionError()
        {
            AnimeManagerApiConnectionError?.Invoke(this, EventArgs.Empty);
        }

        public static async Task InitializeAsync()
        {
            if (DbConnection == null)
                DbConnection = await Database.GetConnection();

            if (!Application.Current.Properties.ContainsKey(AnimeManagerLastUpdatedListPagePropertyKey))
                Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 1;
            await Application.Current.SavePropertiesAsync();

            var api = ((App) Application.Current).DiContainer.Resolve<IAnimeApi>();
            var settings = new Dictionary<string, object>();
            foreach (var setting in Application.Current.Properties.Where(prop => prop.Key.Contains(api.GetType().FullName)))
            {
                var key = setting.Key.Replace(api.GetType().FullName, "");
                settings.Add(key, setting.Value);
            }

            api.Settings = settings;
            await api.Initialize();
            foreach (var setting in api.Settings)
                Application.Current.Properties[api.GetType().FullName + setting.Key] = setting.Value;
            await Application.Current.SavePropertiesAsync();
            Instance = new AnimeManager {Api = api};
        }

        private async Task UpdateCachedAnimesByApi()
        {
            while (true)
            {
                var currentListPage = (int) Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey];
                var result = await Api.GetAnimesByListPageNumberAsync(currentListPage);
                var animes = new List<Anime>(result);
                if (animes.Count > 0)
                {
                    await DbConnection.InsertOrIgnoreAllAsync(animes);
                    Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = currentListPage + 1;
                    await Application.Current.SavePropertiesAsync();
                    OnCachedAnimesUpdated(animes, currentListPage);
                    //Device.BeginInvokeOnMainThread(() => OnCachedAnimesUpdated(animes));
                }
                else
                {
                    OnFinishedCachingAnimes();
                    break;
                }
            }
        }

        private void UpdateCachedAnimesByApiThrowedException(Task task)
        {
            var ex = task.Exception;
            OnAnimeManagerApiConnectionError();
        }

        public async Task<List<Anime>> GetAnimeListAsync()
        {
            var animes = await DbConnection.Table<Anime>().ToListAsync();
#pragma warning disable 4014
            UpdateCachedAnimesByApi().ContinueWith(UpdateCachedAnimesByApiThrowedException, TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
            return animes;
        }

        public async Task<Anime> GetFullAnimeInformation(Anime anime)
        {
            var animes = await DbConnection.GetAllWithChildrenAsync<Anime>(dto => dto.Name == anime.Name);
            var a = animes.First();
            //a.Episodes = await DbConnection.Table<Episode>().Where(ep => ep.AnimeId)
            if (a.ContainsAllInformation)
                return a;

            a = await Api.GetFullAnimeInformationByPageUrlAsync(anime.PageUrl);
            // Assign same id to overwrite current cached anime
            a.Id = anime.Id;

            // TEMPORARY SINCE PAGEURL IS NULL
            a.PageUrl = anime.PageUrl;
            a.ContainsAllInformation = true;
            await DbConnection.InsertAllWithChildrenAsync(a.Episodes);
            await UpdateAnimeInformationForCachedAnime(a);
            return a;
        }

        public async Task UpdateAnimeInformationForCachedAnime(Anime anime)
        {
            await DbConnection.UpdateWithChildrenAsync(anime);
            //await DbConnection.UpdateAsync(anime);
            //await DbConnection.UpdateAllAsync(anime.Episodes);
        }

        public async Task RemoveCache()
        {
            await DbConnection.DeleteAllAsync<Anime>();
            await DbConnection.DeleteAllAsync<Episode>();
            Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 1;
            await Application.Current.SavePropertiesAsync();
        }

        public async Task<IEnumerable<VideoSource>> GetVideoSourcesByEpisode(Episode episode)
        {
            return await Api.GetVideoSourcesByEpisodeUrlAsync(episode.EpisodeUrl);
        }
    }
}