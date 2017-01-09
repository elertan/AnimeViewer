using System;
using System.Linq;
using Acr.UserDialogs;
using AnimeViewer.Models;
using AnimeViewer.Services;
using AnimeViewer.ViewModels;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AnimePage
    {
        private readonly AnimePageViewModel _viewModel;
        private bool _firstAppearance = true;

        public AnimePage(Anime anime, bool hasConnectionIssue = false)
        {
            InitializeComponent();
            _viewModel = new AnimePageViewModel {Anime = anime, HasConnectionIssue = hasConnectionIssue}; //Image and Anime Name
            _viewModel.AllInformationLoaded += _viewModel_AllInformationLoaded;
            BindingContext = _viewModel;
        }

        private async void _viewModel_AllInformationLoaded(object sender, EventArgs e)
        {
            ListView.HeightRequest = ListView.RowHeight*_viewModel.Anime.Episodes.Count;
        }

        private async void AnimePage_OnAppearing(object sender, EventArgs e)
        {
            if (!_firstAppearance) return;
            _firstAppearance = false;

            CustomBackgroundImage.Source = _viewModel.Anime.ImageUrl;
            if (!_viewModel.HasConnectionIssue)
                await _viewModel.GetAllAnimeInformationAsync();
        }

        private async void Episode_Tapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            UserDialogs.Instance.ShowLoading("Loading Episode");
            ((ListView) sender).SelectedItem = null;
            if (e.Item == null) return;
            var episode = (Episode) e.Item;
            var sources = await AnimeManager.Instance.GetVideoSourcesByEpisode(episode);

            var sourceUrl = sources?.First()?.SourceUrl;
            if (sourceUrl == null) return;


            if (!episode.HasWatched)
                await _viewModel.SetEpisodeAsWatched(episode);

            var videoPlayer = DependencyService.Get<IVideoPlayer>();
            videoPlayer.Play(sourceUrl);

            UserDialogs.Instance.HideLoading();
        }
    }
}