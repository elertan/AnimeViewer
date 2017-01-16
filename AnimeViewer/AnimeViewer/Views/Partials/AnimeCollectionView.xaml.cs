using System.Collections;
using AnimeViewer.Models;
using Xamarin.Forms;

namespace AnimeViewer.Views.Partials
{
    public partial class AnimeCollectionView : ContentView
    {
        public static readonly BindableProperty AnimesProperty = BindableProperty.Create(nameof(Animes), typeof(IEnumerable), null);

        public AnimeCollectionView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public IEnumerable Animes
        {
            get { return (IEnumerable) GetValue(AnimesProperty); }
            set { SetValue(AnimesProperty, value); }
        }

        private async void FlowListView_OnFlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Get the corrosponding anime that was tapped
            var anime = (Anime) e.Item;
            // Show the anime page for that anime
            await Navigation.PushAsync(new AnimePage(anime));
        }
    }
}