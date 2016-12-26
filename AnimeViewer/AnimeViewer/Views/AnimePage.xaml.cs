using System;
using System.Linq;
using AnimeViewer.Models;
using AnimeViewer.Services;
using AnimeViewer.ViewModels;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AnimePage
    {
        private readonly AnimePageViewModel _viewModel;

        public AnimePage(Anime anime, bool hasConnectionIssue = false)
        {
            InitializeComponent();
            _viewModel = new AnimePageViewModel {Anime = anime, HasConnectionIssue = hasConnectionIssue}; //Image and Anime Name
            BindingContext = _viewModel;
        }

        private async void AnimePage_OnAppearing(object sender, EventArgs e)
        {
            CustomBackgroundImage.Source = _viewModel.Anime.ImageUrl;
            if (!_viewModel.HasConnectionIssue)
                await _viewModel.GetAllAnimeInformationAsync();
        }

        private async void Episode_Tapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
            var episode = (Episode) e.Item;
            var sources = await AnimeManager.Instance.GetVideoSourcesByEpisode(episode);
            var sourceUrl = sources.First().SourceUrl;
            //await Navigation.PushAsync(new VideoPlayerPage(sourceUrl));
            var videoPlayer = DependencyService.Get<IVideoPlayer>();
            videoPlayer.Play(sourceUrl);
        }
    }
}