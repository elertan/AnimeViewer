using System;
using AnimeViewer.ViewModels;
using NineAnimeApi.Models;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AnimePage : ContentPage
    {
        private readonly AnimePageViewModel _viewModel;

        public AnimePage(Anime anime)
        {
            InitializeComponent();
            _viewModel = new AnimePageViewModel {Anime = anime};
            BindingContext = _viewModel;
        }

        private async void AnimePage_OnAppearing(object sender, EventArgs e)
        {
            await _viewModel.GetAllAnimeInformationAsync();
        }
    }
}