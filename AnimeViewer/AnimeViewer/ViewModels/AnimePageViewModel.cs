using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AnimeViewer.Models;
using MvvmHelpers;

namespace AnimeViewer.ViewModels
{
    public class AnimePageViewModel : BaseViewModel
    {
        private Anime _anime;
        private bool _hasConnectionIssue;

        public AnimePageViewModel()
        {
            PropertyChanged += AnimePageViewModel_PropertyChanged;
        }

        public Anime Anime
        {
            get { return _anime; }
            set
            {
                _anime = value;
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

        public event EventHandler AllInformationLoaded;

        private void OnAllInformationLoaded()
        {
            AllInformationLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void AnimePageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public async Task GetAllAnimeInformationAsync()
        {
            Anime = await AnimeManager.Instance.GetFullAnimeInformation(Anime);
            OnAllInformationLoaded();
        }

        public async Task SetEpisodeAsWatched(Episode episode)
        {
            episode.HasWatched = true;
            await AnimeManager.Instance.UpdateAnimeInformationForCachedAnime(episode.Anime);
        }

        public async Task ClearWatchedIndicators()
        {
            foreach (var episode in Anime.Episodes)
                episode.HasWatched = false;
            await AnimeManager.Instance.UpdateAnimeInformationForCachedAnime(Anime);
        }

        public void UpdateEpisodes()
        {
        }
    }
}