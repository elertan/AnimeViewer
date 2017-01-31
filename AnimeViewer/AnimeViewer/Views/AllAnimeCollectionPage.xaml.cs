using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using AnimeViewer.ViewModels;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AllAnimeCollectionPage : ContentPage
    {
        private readonly AdvancedSearchOptionsViewModel _advancedSearchOptionsViewModel;
        // The viewmodel
        private readonly AllAnimeCollectionPageViewModel _viewModel;
        // Is this the first time the page is appearing
        private bool _firstTimeAppearing = true;
        private bool _scrollBackToTopAfterCachingProcess;

        public AllAnimeCollectionPage()
        {
            InitializeComponent();
            // Create a new viewmodel and apply it as bindingcontext
            _advancedSearchOptionsViewModel = new AdvancedSearchOptionsViewModel();
            _viewModel = new AllAnimeCollectionPageViewModel(_advancedSearchOptionsViewModel);
            BindingContext = _viewModel;
            AdvancedSearchOptionsView.BindingContext = _advancedSearchOptionsViewModel;
        }

        private void Instance_CachedAnimesUpdated(object sender, AnimesUpdatedEventArgs e)
        {
            if (!_scrollBackToTopAfterCachingProcess) _scrollBackToTopAfterCachingProcess = true;
            var last = FlowListView.ItemsSource.Cast<object>().LastOrDefault();
            FlowListView.ScrollTo(last, ScrollToPosition.MakeVisible, false);
        }

        /// <summary>
        ///     Gets invoked when the page is appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // If the page is not appearing for the first time, just return
            if (_firstTimeAppearing)
            {
                // Setup disclosure image
                SearchBarDisclosureImage.Transformations = new List<ITransformation>
                {
                    new RotateTransformation(90),
                    new ColorSpaceTransformation(FFColorMatrix.InvertColorMatrix),
                    new CropTransformation(2.5, 0, 0)
                };

                // Initialize the viewmodel, or whatever this viewmodel might be doing
                await _viewModel.InitializeAsync();
                AnimeManager.Instance.CachedAnimesUpdated += Instance_CachedAnimesUpdated;
                AnimeManager.Instance.FinishedCachingAnimes += Instance_FinishedCachingAnimes;
                _firstTimeAppearing = false;
            }

            await _advancedSearchOptionsViewModel.ApplyFilterAsync(_viewModel.AllAnimes);
        }

        private void Instance_FinishedCachingAnimes(object sender, EventArgs e)
        {
            if (_scrollBackToTopAfterCachingProcess)
            {
                _scrollBackToTopAfterCachingProcess = false;
                var first = FlowListView.ItemsSource.Cast<object>().FirstOrDefault();
                FlowListView.ScrollTo(first, ScrollToPosition.MakeVisible, false);
            }
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
            // Show a loading dialog
            UserDialogs.Instance.ShowLoading("Loading Anime");

            // Get the corrosponding anime that was tapped
            var anime = (Anime) e.Item;
            // Show the anime page for that anime
            await Navigation.PushAsync(new AnimePage(anime, _viewModel.HasConnectionIssue));
        }

        /// <summary>
        ///     Gets invoked when the has a connection issue frame has been tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HasConnectionIssueFrame_Tapped(object sender, EventArgs e)
        {
            // Retries the connection
            await _viewModel.RetryConnection();
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
                // Apply filter
                tasks.Add(_advancedSearchOptionsViewModel.ApplyFilterAsync(_viewModel.AllAnimes));
                tasks.Add(AdvancedSearchOptionsView.FadeTo(0, 250U, Easing.CubicInOut));
                tasks.Add(SearchBarDisclosureImage.RotateTo(0, 250U, Easing.CubicInOut));
                // Wait on all (animation) tasks
                await Task.WhenAll(tasks);

                AdvancedSearchOptionsView.IsVisible = false;
            }
        }
    }
}