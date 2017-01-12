using System;
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
            _viewModel = new AnimePageViewModel {Anime = anime, HasConnectionIssue = hasConnectionIssue}; //Image and Anime Name
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

            // Grab the first video source and return if there is none
            var sourceUrl = sources?.First()?.SourceUrl;
            if (sourceUrl == null) return;

            // Set the episode as watched if it is not yet
            if (!episode.HasWatched)
                await _viewModel.SetEpisodeAsWatched(episode);

            // Get the videoplayer (platform specific, iOS and android has their own 'videoplayer')
            var videoPlayer = DependencyService.Get<IVideoPlayer>();
            // Try playing the video, if it fails tell the user to download a 3rd party videoplayer (on android)
            if (!videoPlayer.Play(sourceUrl)) await UserDialogs.Instance.ConfirmAsync("No video player found on your device, please install one via the playstore.", "VideoPlayer Required", "OK");

            // Hide the loading dialog
            UserDialogs.Instance.HideLoading();
        }
    }
}