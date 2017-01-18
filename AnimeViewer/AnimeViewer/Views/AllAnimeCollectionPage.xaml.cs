using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using AnimeViewer.Models;
using AnimeViewer.ViewModels;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AllAnimeCollectionPage : ContentPage
    {
        // The viewmodel
        private readonly AllAnimeCollectionPageViewModel _viewModel;
        // Is this the first time the page is appearing
        private bool _firstTimeAppearing = true;

        public AllAnimeCollectionPage()
        {
            InitializeComponent();
            // Create a new viewmodel and apply it as bindingcontext
            _viewModel = new AllAnimeCollectionPageViewModel();
            BindingContext = _viewModel;
        }

        /// <summary>
        ///     Gets invoked when the page is appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // If the page is not appearing for the first time, just return
            if (!_firstTimeAppearing) return;
            // Setup disclosure image
            SearchBarDisclosureImage.Transformations = new List<ITransformation>
            {
                new RotateTransformation(90),
                new ColorSpaceTransformation(FFColorMatrix.InvertColorMatrix),
                new CropTransformation(2.5, 0, 0)
            };

            // Set to invisible position
            //await AdvancedSearchOptionsView.FadeTo(0, 0);
            //AdvancedSearchOptionsView.IsVisible = false;

            // Initialize the viewmodel, or whatever this viewmodel might be doing
            await _viewModel.InitializeAsync();
            _firstTimeAppearing = false;
        }

        /// <summary>
        ///     Gets invoked when the text changes in the searchbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Set the search query on our viewmodel to change the animes we see on the flowlistview (collectionview)
            await _viewModel.SetSearchQueryAsync(e.NewTextValue);
        }

        /// <summary>
        ///     Gets invoked when an anime is tapped in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FlowListView_OnFlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Get the corrosponding anime that was tapped
            var anime = (Anime) e.Item;
            // Show the anime page for that anime
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

        /// <summary>
        ///     Gets invoked when the has a connection issue frame has been tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HasConnectionIssueFrame_Tapped(object sender, EventArgs e)
        {
            //var accepted = await DisplayAlert("Retry Connection", "Do you want to retry the connection?", "Yes", "No");
            //if (accepted)
            // Retries the connection
            await _viewModel.RetryConnection();
        }

        private void IndicatorLabel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsVisibleProperty.PropertyName)
            {
                var label = (Label) sender;
                var contentView = (ContentView) label.Parent;
                contentView.IsVisible = true;
            }
        }

        private async void SearchBar_OnSearchButtonPressed(object sender, EventArgs e)
        {
            //await _viewModel.SetSearchQueryAsync(SearchBar.Text);
        }

        private async void SearchBarDisclosureImage_Tapped(object sender, EventArgs e)
        {
            var tasks = new List<Task>();
            if (Math.Abs(SearchBarDisclosureImage.Rotation - 180) > 0.5)
            {
                AdvancedSearchOptionsView.IsVisible = true;

                tasks.Add(AdvancedSearchOptionsView.FadeTo(1, 250U, Easing.CubicInOut));
                tasks.Add(SearchBarDisclosureImage.RotateTo(180, 250U, Easing.CubicInOut));
                // Wait on all (animation) tasks
                await Task.WhenAll(tasks);
            }
            else
            {
                tasks.Add(AdvancedSearchOptionsView.FadeTo(0, 250U, Easing.CubicInOut));
                tasks.Add(SearchBarDisclosureImage.RotateTo(0, 250U, Easing.CubicInOut));
                // Wait on all (animation) tasks
                await Task.WhenAll(tasks);

                AdvancedSearchOptionsView.IsVisible = false;
            }
        }
    }
}