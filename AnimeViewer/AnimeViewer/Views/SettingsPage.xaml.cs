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

        private void QualitySelectionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            var quality = picker.Items[picker.SelectedIndex];
        }

        private void SettingsPage_OnAppearing(object sender, EventArgs e)
        {
            QualitySelectionPicker.SelectedIndex = QualitySelectionPicker.Items.IndexOf((string)App.Current.Properties[AppSettingKeys.VideoQuality]);
        }

        private async void SettingsPage_OnDisappearing(object sender, EventArgs e)
        {
            // Video Settings
            App.Current.Properties[AppSettingKeys.VideoQuality] = QualitySelectionPicker.Items[QualitySelectionPicker.SelectedIndex];

            await App.Current.SavePropertiesAsync();
        }
    }
}