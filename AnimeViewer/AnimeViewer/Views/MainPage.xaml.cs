using System;
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
            ((App) Application.Current).Navigation = Detail.Navigation;
        }

        private void MainPage_OnIsPresentedChanged(object sender, EventArgs e)
        {
            //if (IsPresented)
            //    Focus();
        }
    }
}