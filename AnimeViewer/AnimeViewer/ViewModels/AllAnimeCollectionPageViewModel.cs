﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using MvvmHelpers;

namespace AnimeViewer.ViewModels
{
    public class AllAnimeCollectionPageViewModel : BaseViewModel
    {
        private ObservableRangeCollection<Anime> _allAnimes;
        private bool _hasConnectionIssue;
        private bool _isCachingAnimes;
        private bool _isInitializing = true;
        private ObservableRangeCollection<Anime> _visibleAnimes;

        public AllAnimeCollectionPageViewModel()
        {
            AllAnimes = new ObservableRangeCollection<Anime>();
        }

        public bool IsInitializing
        {
            get { return _isInitializing; }
            set
            {
                _isInitializing = value;
                OnPropertyChanged();
            }
        }

        public bool IsCachingAnimes
        {
            get { return _isCachingAnimes; }
            set
            {
                _isCachingAnimes = value;
                OnPropertyChanged();
            }
        }

        public string SearchKeyword { get; private set; }

        public ObservableRangeCollection<Anime> AllAnimes
        {
            get { return _allAnimes; }
            set
            {
                _allAnimes = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<Anime> VisibleAnimes
        {
            get { return _visibleAnimes; }
            set
            {
                _visibleAnimes = value;
                OnPropertyChanged();
            }
        }

        public bool HasConnectionIssue
        {
            get { return _hasConnectionIssue; }
            set
            {
                _hasConnectionIssue = value;
                OnPropertyChanged();
            }
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;
            while (AnimeManager.Instance == null) await Task.Delay(50);
            AnimeManager.Instance.CachedAnimesUpdated += Instance_CachedAnimesUpdated;
            AnimeManager.Instance.FinishedCachingAnimes += Instance_FinishedCachingAnimes;
            AnimeManager.Instance.AnimeManagerApiConnectionError += Instance_AnimeManagerApiConnectionError;
            AnimeManager.Instance.CacheRemoved += Instance_CacheRemoved;
            AllAnimes.AddRange(await AnimeManager.Instance.GetAnimeListAsync());
            VisibleAnimes = AllAnimes;
            IsBusy = false;
        }

        private async void Instance_CacheRemoved(object sender, EventArgs e)
        {
            // Retry caching
            await RetryConnection();
        }

        private void Instance_FinishedCachingAnimes(object sender, EventArgs e)
        {
            IsCachingAnimes = false;
            IsInitializing = false;
        }

        public async Task RetryConnection()
        {
            IsBusy = true;
            HasConnectionIssue = false;
            AllAnimes = new ObservableRangeCollection<Anime>(await AnimeManager.Instance.GetAnimeListAsync());
            VisibleAnimes = AllAnimes;
            await SetSearchQueryAsync(SearchKeyword);
            IsBusy = false;
        }

        public async Task SetSearchQueryAsync(string query)
        {
            while ((VisibleAnimes == null) || (VisibleAnimes.Count == 0)) await Task.Delay(200);
            if (string.IsNullOrWhiteSpace(query))
                SearchKeyword = "";
            else
                SearchKeyword = query.ToLower();
            await ApplySearchQueryAsync();
        }

        private async Task ApplySearchQueryAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
                VisibleAnimes = AllAnimes;
            else
            {
                var animes = await Task.Run(() => new ObservableRangeCollection<Anime>(
                    AllAnimes.Where(anime => anime.Name.ToLower().Contains(SearchKeyword)).ToArray()));
                VisibleAnimes = animes;
            }
        }

        private void Instance_AnimeManagerApiConnectionError(object sender, EventArgs e)
        {
            HasConnectionIssue = true;
        }

        private void Instance_CachedAnimesUpdated(object sender, AnimesUpdatedEventArgs e)
        {
            if (IsInitializing) IsInitializing = false;
            IsCachingAnimes = true;
            AllAnimes.AddRange(e.Animes);
            ApplySearchQueryAsync();
        }

        public async Task RecacheAllAnimes()
        {
            IsBusy = true;
            await AnimeManager.Instance.RemoveCache();
            AllAnimes = new ObservableRangeCollection<Anime>();
            AllAnimes.AddRange(await AnimeManager.Instance.GetAnimeListAsync());
            VisibleAnimes = AllAnimes;
            IsBusy = false;
        }
    }
}