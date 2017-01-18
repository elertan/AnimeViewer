﻿using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using AnimeViewer.Models;
using AnimeViewer.Services;
using AnimeViewer.ViewModels;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AnimePage
    {
        // The view model
        private readonly AnimePageViewModel _viewModel;
        // Is it the first time this page is appearing (leaving the videoplayer causes another OnAppearing call)
        private bool _firstAppearance = true;

        /// <summary>
        ///     Constructs a new animepage
        /// </summary>
        /// <param name="anime">Anime to view</param>
        /// <param name="hasConnectionIssue">Are there connection issues?</param>
        public AnimePage(Anime anime, bool hasConnectionIssue = false)
        {
            InitializeComponent();
            // Create new viewmodel with the given variables
            _viewModel = new AnimePageViewModel {Anime = anime, HasConnectionIssue = hasConnectionIssue};
            //Image and Anime Name
            // Bind event if all information loaded
            _viewModel.AllInformationLoaded += _viewModel_AllInformationLoaded;
            // Apply to binding context so we can bind to the viewmodel's properties in the view
            BindingContext = _viewModel;
        }

        /// <summary>
        ///     Gets invoked when all anime information is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _viewModel_AllInformationLoaded(object sender, EventArgs e)
        {
            // Extend listview height to the amount of elements in the list (No scroll listview, entire length)
            ListView.HeightRequest = ListView.RowHeight*_viewModel.Anime.Episodes.Count;
        }

        /// <summary>
        ///     Gets invoked when the page appears
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AnimePage_OnAppearing(object sender, EventArgs e)
        {
            // Is it the first time this page is appearing (leaving the videoplayer causes another OnAppearing call) only proceed if so
            if (!_firstAppearance) return;
            _firstAppearance = false;

            // Show a loading dialog
            UserDialogs.Instance.ShowLoading("Loading Anime");

            // Set the custom background (blur background) to the anime's poster image
            CustomBackgroundImage.Source = _viewModel.Anime.ImageUrl;
            CustomBackgroundImage.Transformations = new List<ITransformation> {new BlurredTransformation(3)};
            // If there is no connection issue
            //if (!_viewModel.HasConnectionIssue)
            // Get all information async
            await _viewModel.GetAllAnimeInformationAsync();

            // Hide the loading dialog
            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        ///     Gets invoked when an episode is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Episode_Tapped(object sender, ItemTappedEventArgs e)
        {
            // Only proceed if the selected item isn't null (nothing) since we are setting the selecteditem to null this method gets reinvoked again, and we dont want that
            if (e.Item == null) return;

            // Show a loading dialog
            UserDialogs.Instance.ShowLoading("Loading Episode");

            // Remove the item being selected (Reinvokes this method)
            ((ListView) sender).SelectedItem = null;

            // Get the episode
            var episode = (Episode) e.Item;
            // Get all the videosources via our animemanager
            var sources = await AnimeManager.Instance.GetVideoSourcesByEpisode(episode);

            // return if there are no sources
            if ((sources == null) || !sources.Any())
            {
                await UserDialogs.Instance.AlertAsync("Video could not be found on our host");
                return;
            }

            string sourceUrl;
            try
            {
                sourceUrl = sources.First(s => s.Quality == (string) Application.Current.Properties[AppSettingsKeys.VideoQuality]).SourceUrl;
            }
            catch
            {
                sourceUrl = sources.First().SourceUrl;
            }

            // Set the episode as watched if it is not yet
            if (!episode.HasWatched)
            {
                await _viewModel.SetEpisodeAsWatched(episode);
                // Update to show has watched
                ListView.ItemsSource = new List<Episode>(_viewModel.Anime.Episodes);
            }


            // Get the videoplayer (platform specific, iOS and android has their own 'videoplayer')
            var videoPlayer = DependencyService.Get<IVideoPlayer>();
            // Try playing the video, if it fails tell the user to download a 3rd party videoplayer (on android)
            if (!videoPlayer.Play(sourceUrl))
                await
                    UserDialogs.Instance.ConfirmAsync(
                        "No video player found on your device, please install one via the playstore.",
                        "VideoPlayer Required", "OK");

            // Hide the loading dialog
            UserDialogs.Instance.HideLoading();
        }

        private async void Options_Clicked(object sender, EventArgs e)
        {
            const string clearWatchedIndicatorsOption = "Clear watched indicators";
            var clearWatchedEpisodesOption =
                await
                    UserDialogs.Instance.ActionSheetAsync("Options", "Cancel", null, null, clearWatchedIndicatorsOption);

            if (clearWatchedEpisodesOption == clearWatchedIndicatorsOption)
            {
                await _viewModel.ClearWatchedIndicators();

                // Update to remove has watched
                ListView.ItemsSource = new List<Episode>(_viewModel.Anime.Episodes);
            }
        }

        private async void ScrollToLastWatchedEpisode_Clicked(object sender, EventArgs e)
        {
            var lastWatchedEpisode = _viewModel.Anime.Episodes.LastOrDefault(ep => ep.HasWatched);
            if (lastWatchedEpisode == default(Episode))
            {
                await UserDialogs.Instance.AlertAsync("You haven't watched any episode of this anime yet.");
                return;
            } // No episodes watched yet

            await ScrollView.ScrollToAsync(ListView, ScrollToPosition.Start, true);

            var lastWatchedEpisodeIndex = _viewModel.Anime.Episodes.IndexOf(lastWatchedEpisode);
            var listViewOffset = lastWatchedEpisodeIndex*ListView.RowHeight;

            await ScrollView.ScrollToAsync(0, ScrollView.ScrollY + listViewOffset, true);
        }

        private async void FavouriteButton_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.SetAnimeFavouriteStateAsync(!_viewModel.Anime.IsFavourited);
        }
    }
}