﻿using AnimeViewer.Services;
using AnimeViewer.Services.Implementations;
using AnimeViewer.Views;
using DLToolkit.Forms.Controls;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

// Xaml Compilation for a faster app experience

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace AnimeViewer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Initialize the flowlistview (the large list of animes that you see at the first page)
            FlowListView.Init();

            // Set the mainpage
            MainPage = new MainPage();
        }

        // Our container to store losely coupled parts
        public UnityContainer DiContainer { get; set; } = new UnityContainer();

        protected override async void OnStart()
        {
            // First time
            if (!Current.Properties.ContainsKey(AppSettingKeys.NotFirstTimeOpeningApp))
            {
                // Set default video quality
                Current.Properties[AppSettingKeys.VideoQuality] = "720p";
                Current.Properties[AppSettingKeys.AutomaticallyPlayNextEpisode] = true;
                Current.Properties[AppSettingKeys.AutomaticallyPlayNextEpisodeCancellableDelay] = 8000f;

                Current.Properties[AppSettingKeys.NotFirstTimeOpeningApp] = true;
                await Current.SavePropertiesAsync();
            }

            // Register services to get a losely coupled structure in our app (easily being able to swap our api)
            DiContainer.RegisterType<IAnimeApi, KissanimeApi>();

            // Initialize our animemanager (setup api, database)
            await AnimeManager.InitializeAsync();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}