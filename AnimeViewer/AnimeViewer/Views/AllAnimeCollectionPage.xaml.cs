using KissAnime.Models;
using MvvmHelpers;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AllAnimeCollectionPage : ContentPage
    {
        public AllAnimeCollectionPage()
        {
            InitializeComponent();

            Animes.Add(new Anime {Name = "test"});
        }

        public ObservableRangeCollection<Anime> Animes { get; set; } = new ObservableRangeCollection<Anime>();
    }
}