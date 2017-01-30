using System;
using AnimeViewer.ViewModels;

namespace AnimeViewer.Views
{
    public partial class SettingsPage
    {
        private readonly SettingPageViewModel _settingsPageViewModel;

        public SettingsPage()
        {
            _settingsPageViewModel = new SettingPageViewModel();
            BindingContext = _settingsPageViewModel;
            InitializeComponent();
        }

        private async void ClearCacheButton_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Clear cache", "Are you sure you want to clear the cache?", "Yes", "No"))
                await AnimeManager.Instance.RemoveCache();
        }

        private async void SettingsPage_OnDisappearing(object sender, EventArgs e)
        {
            await _settingsPageViewModel.SaveSettings();
        }
    }
}