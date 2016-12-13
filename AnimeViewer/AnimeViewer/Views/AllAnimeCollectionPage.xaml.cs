using AnimeViewer.ViewModels;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AllAnimeCollectionPage : ContentPage
    {
        private readonly AllAnimeCollectionPageViewModel _viewModel;

        public AllAnimeCollectionPage()
        {
            InitializeComponent();
            _viewModel = new AllAnimeCollectionPageViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SetSearchQuery(e.NewTextValue);
        }
    }
}