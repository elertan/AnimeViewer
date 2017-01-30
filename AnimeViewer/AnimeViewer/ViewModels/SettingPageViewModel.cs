using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MvvmHelpers;
using Xamarin.Forms;

namespace AnimeViewer.ViewModels
{
    public class SettingPageViewModel : BaseViewModel
    {
        private int _autoPlayDuration;
        private bool _autoPlayEnabled;
        private string _videoQuality;
        public List<string> VideoQualities = new List<string> {"1080p", "720p", "480p", "360p", "240p"};

        public SettingPageViewModel()
        {
            PropertyChanged += SettingPageViewModel_PropertyChanged;
            //init properties
            AutoPlayEnabled = (bool) Application.Current.Properties[AppSettingKeys.AutomaticallyPlayNextEpisode];
            AutoPlayDuration =
                (int) Application.Current.Properties[AppSettingKeys.AutomaticallyPlayNextEpisodeCancellableDelay];
            VideoQuality = (string) Application.Current.Properties[AppSettingKeys.VideoQuality];
        }

        public int SelectedVideoQualityItemIndex
        {
            get { return VideoQualities.IndexOf(VideoQuality); }
            set { VideoQuality = VideoQualities[value]; }
        }

        public bool AutoPlayEnabled
        {
            get { return _autoPlayEnabled; }
            set
            {
                _autoPlayEnabled = value;
                OnPropertyChanged();
            }
        }

        public int AutoPlayDuration
        {
            get { return _autoPlayDuration; }
            set
            {
                _autoPlayDuration = value;
                OnPropertyChanged();
            }
        }

        public string VideoQuality
        {
            get { return _videoQuality; }
            set
            {
                _videoQuality = value;
                OnPropertyChanged();
            }
        }

        public async Task SaveSettings()
        {
            Application.Current.Properties[AppSettingKeys.AutomaticallyPlayNextEpisode] = AutoPlayEnabled;
            Application.Current.Properties[AppSettingKeys.AutomaticallyPlayNextEpisodeCancellableDelay] =
                AutoPlayDuration;
            Application.Current.Properties[AppSettingKeys.VideoQuality] = VideoQuality;

            await Application.Current.SavePropertiesAsync();
        }

        private void SettingPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}