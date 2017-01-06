using System;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(true);
        }
    }
}