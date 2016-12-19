using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using NineAnimeApi;
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

        public Api Api { get; set; }

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

            Instance = new AnimeManager {Api = new Api()};
        }

        private async Task UpdateCachedAnimesByApi()
        {
            while (true)
            {
                var currentListPage = (int) Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey];
                var animes = await Api.GetAnimesAscendingOrderByPageAsync(currentListPage);
                if (animes.Count > 0)
                {
                    await DbConnection.InsertOrIgnoreAllAsync(animes.ToAnimeDtos());
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
            var animeDtos = await DbConnection.Table<AnimeDto>().ToListAsync();
#pragma warning disable 4014
            UpdateCachedAnimesByApi().ContinueWith(UpdateCachedAnimesByApiThrowedException, TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
            return animeDtos.ToAnimes();
        }

        public async Task<Anime> GetFullAnimeInformation(Anime anime)
        {
            var animeDto = await DbConnection.Table<AnimeDto>().Where(dto => dto.Name == anime.Name).FirstAsync();
            if (animeDto.ContainsAllInformation)
                return animeDto.ToAnime();

            var fullAnime = await Api.GetAnimeByPageUrl(anime.PageUrl);
            animeDto.Summary = fullAnime.Summary;
            animeDto.ContainsAllInformation = true;
            await DbConnection.UpdateAsync(animeDto);
            return fullAnime;
        }

        public async Task RemoveCache()
        {
            await DbConnection.DeleteAllAsync<AnimeDto>();
            await DbConnection.DeleteAllAsync<EpisodeDto>();
            Application.Current.Properties[AnimeManagerLastUpdatedListPagePropertyKey] = 1;
            await Application.Current.SavePropertiesAsync();
        }
    }
}