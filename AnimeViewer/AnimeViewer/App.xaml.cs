using AnimeViewer.Services;
using AnimeViewer.Services.Implementations;
using AnimeViewer.Views;
using DLToolkit.Forms.Controls;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace AnimeViewer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            FlowListView.Init();

            MainPage = new MainPage();
        }

        public UnityContainer DiContainer { get; set; } = new UnityContainer();
        public INavigation Navigation { get; set; }

        protected override async void OnStart()
        {
            // Handle when your app starts

            // Register services
            DiContainer.RegisterType<IAnimeApi, KissanimeApi>();

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