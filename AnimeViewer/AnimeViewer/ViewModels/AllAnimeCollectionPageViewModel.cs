using System;
using System.Linq;
using System.Threading.Tasks;
using AnimeViewer.EventArguments;
using MvvmHelpers;
using NineAnimeApi.Models;

namespace AnimeViewer.ViewModels
{
    public class AllAnimeCollectionPageViewModel : BaseViewModel
    {
        private ObservableRangeCollection<Anime> _allAnimes;
        private float _cachingAnimeProgress;
        private int _currentCachedAnimePageNumber;
        private bool _hasConnectionIssue;
        private bool _isCachingAnimes = true;
        private ObservableRangeCollection<Anime> _visibleAnimes;

        public AllAnimeCollectionPageViewModel()
        {
            AllAnimes = new ObservableRangeCollection<Anime>();
        }

        public int TotalAnimePages => 164;

        public int CurrentCachedAnimePageNumber
        {
            get { return _currentCachedAnimePageNumber; }
            set
            {
                _currentCachedAnimePageNumber = value;
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

        public string CachingAnimeText => $"Caching {CurrentCachedAnimePageNumber} of {TotalAnimePages}";

        public float CachingAnimeProgress
        {
            get { return _cachingAnimeProgress; }
            set
            {
                _cachingAnimeProgress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CachingAnimeText));
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
            await AnimeManager.InitializeAsync();
            AnimeManager.Instance.CachedAnimesUpdated += Instance_CachedAnimesUpdated;
            AnimeManager.Instance.FinishedCachingAnimes += Instance_FinishedCachingAnimes;
            AnimeManager.Instance.AnimeManagerApiConnectionError += Instance_AnimeManagerApiConnectionError;
            AllAnimes.AddRange(await AnimeManager.Instance.GetAnimeListAsync());
            VisibleAnimes = AllAnimes;
            IsBusy = false;
        }

        private void Instance_FinishedCachingAnimes(object sender, EventArgs e)
        {
            IsCachingAnimes = false;
        }

        public async Task RetryConnection()
        {
            IsBusy = true;
            HasConnectionIssue = false;
            AllAnimes = new ObservableRangeCollection<Anime>(await AnimeManager.Instance.GetAnimeListAsync());
            VisibleAnimes = AllAnimes;
            SetSearchQuery(SearchKeyword);
            IsBusy = false;
        }

        public void SetSearchQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                SearchKeyword = "";
            else
                SearchKeyword = query.ToLower();
            ApplySearchQuery();
        }

        private void ApplySearchQuery()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
                VisibleAnimes = AllAnimes;
            else
                VisibleAnimes = new ObservableRangeCollection<Anime>(AllAnimes.Where(anime => anime.Name.ToLower().Contains(SearchKeyword)).ToArray());
        }

        private void Instance_AnimeManagerApiConnectionError(object sender, EventArgs e)
        {
            HasConnectionIssue = true;
        }

        private void Instance_CachedAnimesUpdated(object sender, AnimesUpdatedEventArgs e)
        {
            IsCachingAnimes = true;
            CurrentCachedAnimePageNumber = e.Page;
            CachingAnimeProgress = Convert.ToSingle(e.Page)/Convert.ToSingle(TotalAnimePages);
            AllAnimes.AddRange(e.Animes);
            ApplySearchQuery();
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