using System;

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
    }
}