using System.ComponentModel;
using System.Threading.Tasks;
using MvvmHelpers;
using NineAnimeApi.Models;

namespace AnimeViewer.ViewModels
{
    public class AnimePageViewModel : BaseViewModel
    {
        private Anime _anime;

        public AnimePageViewModel()
        {
            PropertyChanged += AnimePageViewModel_PropertyChanged;
        }

        public Anime Anime
        {
            get { return _anime; }
            set
            {
                _anime = value;
                OnPropertyChanged();
            }
        }

        private void AnimePageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public async Task GetAllAnimeInformationAsync()
        {
            Anime = await AnimeManager.Instance.GetFullAnimeInformation(Anime);
        }
    }
}