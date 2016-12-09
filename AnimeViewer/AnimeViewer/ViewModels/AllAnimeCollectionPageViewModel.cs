using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KissAnime.Models;
using MvvmHelpers;

namespace AnimeViewer.ViewModels
{
    public class AllAnimeCollectionPageViewModel : INotifyPropertyChanged
    {
        private ObservableRangeCollection<Anime> _animes;

        public AllAnimeCollectionPageViewModel()
        {
            Animes = new ObservableRangeCollection<Anime>();
        }

        public ObservableRangeCollection<Anime> Animes
        {
            get { return _animes; }
            set
            {
                _animes = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task InitializeAsync()
        {
            //await AnimeManager
        }
    }
}