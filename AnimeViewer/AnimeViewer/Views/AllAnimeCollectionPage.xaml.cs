using System;
using AnimeViewer.Models;
using AnimeViewer.ViewModels;
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

            if (!_firstTimeAppearing) return;
            await _viewModel.InitializeAsync();
            _firstTimeAppearing = false;
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SetSearchQuery(e.NewTextValue);
        }

        private async void FlowListView_OnFlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            var anime = (Anime) e.Item;
            await Navigation.PushAsync(new AnimePage(anime, _viewModel.HasConnectionIssue));
        }

        private void FlowListView_OnTapped(object sender, EventArgs e)
        {
            // TODO: DOES NOT WORK
            //SearchBar.Unfocus();
        }

        private void ListView_OnRefreshing(object sender, EventArgs e)
        {
            //await _viewModel.RecacheAllAnimes();
        }

        private async void HasConnectionIssueFrame_Tapped(object sender, EventArgs e)
        {
            //var accepted = await DisplayAlert("Retry Connection", "Do you want to retry the connection?", "Yes", "No");
            //if (accepted)
            await _viewModel.RetryConnection();
        }
    }
}