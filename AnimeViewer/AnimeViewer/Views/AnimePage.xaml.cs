using System;
using AnimeViewer.ViewModels;
using NineAnimeApi.Models;

namespace AnimeViewer.Views
{
    public partial class AnimePage
    {
        private readonly AnimePageViewModel _viewModel;

        public AnimePage(Anime anime)
        {
            InitializeComponent();
            _viewModel = new AnimePageViewModel {Anime = anime}; //Image and Anime Name
            BindingContext = _viewModel;
        }

        private async void AnimePage_OnAppearing(object sender, EventArgs e)
        {
            BackgroundImage.Source = _viewModel.Anime.PosterImageUrl;
            await _viewModel.GetAllAnimeInformationAsync();
        }
    }
}