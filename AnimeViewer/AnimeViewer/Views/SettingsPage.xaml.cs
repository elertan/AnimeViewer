using System;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void ClearCacheButton_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Clear cache", "Are you sure you want to clear the cache?", "Yes", "No"))
                await AnimeManager.Instance.RemoveCache();
        }
    }
}