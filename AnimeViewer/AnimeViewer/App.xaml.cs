using AnimeViewer.Views;
using DLToolkit.Forms.Controls;
using Xamarin.Forms;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace AnimeViewer
{
    public partial class App : Application
    {
        public INavigation Navigation { get; set; }

        public App()
        {
            InitializeComponent();

            FlowListView.Init();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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