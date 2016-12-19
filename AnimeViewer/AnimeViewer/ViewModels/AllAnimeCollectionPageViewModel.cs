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

        private bool _hasConnectionIssue;
        private ObservableRangeCollection<Anime> _visibleAnimes;

        public AllAnimeCollectionPageViewModel()
        {
            AllAnimes = new ObservableRangeCollection<Anime>();
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
            AnimeManager.Instance.AnimeManagerApiConnectionError += Instance_AnimeManagerApiConnectionError;
            AllAnimes.AddRange(await AnimeManager.Instance.GetAnimeListAsync());
            VisibleAnimes = AllAnimes;
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