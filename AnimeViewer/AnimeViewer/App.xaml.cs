using AnimeViewer.Views;
using DLToolkit.Forms.Controls;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace AnimeViewer
{
    public partial class App
    {
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