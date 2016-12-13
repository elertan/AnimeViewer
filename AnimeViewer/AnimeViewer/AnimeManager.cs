using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using NineAnimeApi.Models;
using SQLite.Net.Async;
using Xamarin.Forms;

namespace AnimeViewer
{
    public class AnimeManager
    {
        private static readonly string AnimeManagerLastUpdatedListPagePropertyKey = "AnimeManagerLastUpdatedListPage";
        public static SQLiteAsyncConnection DbConnection { get; set; }
        public static AnimeManager Instance { get; set; }

        public event EventHandler<AnimesUpdatedEventArgs> CachedAnimesUpdated;
        public event EventHandler AnimeManagerApiConnectionError;
        public NineAnimeApi.Api Api
        {
            get;
            set;
        }

        private void OnCachedAnimesUpdated(List<Anime> addedAnimes)
        {
            CachedAnimesUpdated?.Invoke(this, new AnimesUpdatedEventArgs { Animes = addedAnimes });
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
                Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 0;
            await Application.Current.SavePropertiesAsync();

            Instance = new AnimeManager();
            Instance.Api = new NineAnimeApi.Api();
        }

        private async Task UpdateCachedAnimesByApi()
        {
            while (true)
            {
                var currentListPage = (int)Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey];
                var animes = await Api.GetAnimesAscendingOrderByPageAsync(currentListPage);
                if (animes.Count > 0)
                {
                    await DbConnection.InsertOrIgnoreAllAsync(animes.ToAnimeDtos());
                    Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = currentListPage + 1;
                    await Application.Current.SavePropertiesAsync();
                    OnCachedAnimesUpdated(animes);
                    //Device.BeginInvokeOnMainThread(() => OnCachedAnimesUpdated(animes));
                }
                else
                {
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
            var animeDtos = await DbConnection.Table<AnimeDto>().ToListAsync();
            UpdateCachedAnimesByApi().ContinueWith(UpdateCachedAnimesByApiThrowedException, TaskContinuationOptions.OnlyOnFaulted);
            return animeDtos.ToAnimes();
        }
    }
}