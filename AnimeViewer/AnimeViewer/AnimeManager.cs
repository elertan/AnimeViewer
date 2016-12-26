using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using AnimeViewer.Services;
using Microsoft.Practices.Unity;
using SQLite.Net.Async;
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

            Instance = new AnimeManager {Api = ((App) Application.Current).DiContainer.Resolve<IAnimeApi>()};
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
            var a = await DbConnection.Table<Anime>().Where(dto => dto.Name == anime.Name).FirstAsync();
            if (a.ContainsAllInformation)
                return a;

            a = await Api.GetFullAnimeInformationByPageUrlAsync(anime.PageUrl);
            await DbConnection.UpdateAsync(a);
            return a;
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