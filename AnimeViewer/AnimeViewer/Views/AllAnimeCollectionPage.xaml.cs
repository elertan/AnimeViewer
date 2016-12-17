using System;
using AnimeViewer.ViewModels;
using NineAnimeApi.Models;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AllAnimeCollectionPage : ContentPage
    {
        private readonly AllAnimeCollectionPageViewModel _viewModel;
        private bool _firstTimeAppearing = true;

        public AllAnimeCollectionPage()
        {
            InitializeComponent();
            _viewModel = new AllAnimeCollectionPageViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_firstTimeAppearing)
            {
                await _viewModel.InitializeAsync();
                _firstTimeAppearing = false;
            }
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SetSearchQuery(e.NewTextValue);
        }

        private async void FlowListView_OnFlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            var anime = (Anime) e.Item;
            await App.Navigation.PushAsync(new AnimePage(anime));
        }

        private void FlowListView_OnTapped(object sender, EventArgs e)
        {
            // TODO: DOES NOT WORK
            //SearchBar.Unfocus();
        }
    }
}