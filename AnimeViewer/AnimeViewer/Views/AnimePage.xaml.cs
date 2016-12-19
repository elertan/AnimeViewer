using System;
using AnimeViewer.ViewModels;
using NineAnimeApi.Models;

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
            BackgroundImage.Source = _viewModel.Anime.PosterImageUrl;
            if (!_viewModel.HasConnectionIssue)
                await _viewModel.GetAllAnimeInformationAsync();
        }
    }
}