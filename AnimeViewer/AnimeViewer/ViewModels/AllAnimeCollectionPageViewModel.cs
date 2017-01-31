using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using MvvmHelpers;

namespace AnimeViewer.ViewModels
{
    public class AllAnimeCollectionPageViewModel : BaseViewModel
    {
        private List<Anime> _allAnimes;
        private ObservableRangeCollection<Anime> _filteredAnimes;
        private bool _hasConnectionIssue;
        private bool _isCachingAnimes;
        private bool _isInitializing = true;
        private ObservableRangeCollection<Anime> _visibleAnimes;

        public AllAnimeCollectionPageViewModel(AdvancedSearchOptionsViewModel filterModel)
        {
            FilterModel = filterModel;
            FilterModel.AnimesFiltered += FilterModel_AnimesFiltered;
            AllAnimes = new List<Anime>();
            FilteredAnimes = new ObservableRangeCollection<Anime>(AllAnimes);

            //AnimeManager.Instance.UpdatedCachedAnimeInformation += Instance_UpdatedCachedAnimeInformation;
        }

        public Action AfterAnimeCachedUpdatedAction { get; set; }
        public AdvancedSearchOptionsViewModel FilterModel { get; set; }

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

        public List<Anime> AllAnimes
        {
            get { return _allAnimes; }
            set
            {
                _allAnimes = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<Anime> FilteredAnimes
        {
            get { return _filteredAnimes; }
            set
            {
                _filteredAnimes = value;
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

        private async void FilterModel_AnimesFiltered(object sender, AnimesFilteredEventArgs e)
        {
            FilteredAnimes = new ObservableRangeCollection<Anime>(e.Animes);
            VisibleAnimes = new ObservableRangeCollection<Anime>(await ApplySearchQueryAsync());
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
            VisibleAnimes = FilteredAnimes;
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
            AllAnimes = await AnimeManager.Instance.GetAnimeListAsync();
            VisibleAnimes = new ObservableRangeCollection<Anime>(AllAnimes);
            await SetSearchQueryAsync(SearchKeyword);
            IsBusy = false;
        }

        public async Task SetSearchQueryAsync(string query)
        {
            SearchKeyword = string.IsNullOrWhiteSpace(query) ? "" : query.ToLower();
            VisibleAnimes = new ObservableRangeCollection<Anime>(await ApplySearchQueryAsync());
        }

        public async Task<IEnumerable<Anime>> ApplySearchQueryAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
                return FilteredAnimes;

            var animes = await Task.Run(() => new ObservableRangeCollection<Anime>(
                    FilteredAnimes.Where(anime => anime.Name.ToLower().Contains(SearchKeyword)).ToArray())
            );
            return animes;
        }

        private void Instance_AnimeManagerApiConnectionError(object sender, EventArgs e)
        {
            HasConnectionIssue = true;
        }

        private async void Instance_CachedAnimesUpdated(object sender, AnimesUpdatedEventArgs e)
        {
            if (IsInitializing) IsInitializing = false;
            IsCachingAnimes = true;
            //foreach (var anime in e.Animes)
            //{
            //    if (AllAnimes.FirstOrDefault(a => a.Name == anime.Name && a.Language == anime.Language) != null) AllAnimes.r
            //}
            //AllAnimes.AddRange(e.Animes);
            AllAnimes.AddRange(e.Animes);
            FilteredAnimes = new ObservableRangeCollection<Anime>(AllAnimes);
            VisibleAnimes = new ObservableRangeCollection<Anime>(await ApplySearchQueryAsync());

            AfterAnimeCachedUpdatedAction?.Invoke();
        }

        public async Task RecacheAllAnimes()
        {
            IsBusy = true;
            await AnimeManager.Instance.RemoveCache();
            AllAnimes = new List<Anime>();
            AllAnimes.AddRange(await AnimeManager.Instance.GetAnimeListAsync());
            VisibleAnimes = new ObservableRangeCollection<Anime>(AllAnimes);
            IsBusy = false;
        }
    }
}