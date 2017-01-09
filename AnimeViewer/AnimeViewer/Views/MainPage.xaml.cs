using System;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_OnAppearing(object sender, EventArgs e)
        {
        }

        private void MainPage_OnIsPresentedChanged(object sender, EventArgs e)
        {
            //if (IsPresented)
            //    Focus();
        }

        private void NavigationPage_OnAppearing(object sender, EventArgs e)
        {
            // Register global navigation
            ((App) Application.Current).DiContainer.RegisterInstance(typeof(INavigation), NavigationPage.Navigation);
        }
    }
}