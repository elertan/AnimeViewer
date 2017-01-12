using System.Collections.Generic;
using AnimeViewer.Models;
using AnimeViewer.ViewModels;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AnimeCollectionPage
    {
        private readonly AnimeCollectionPageViewModel _viewModel;

        public AnimeCollectionPage(IEnumerable<Anime> animes)
        {
            InitializeComponent();

            _viewModel = new AnimeCollectionPageViewModel();
            _viewModel.Animes = animes;
            BindingContext = _viewModel;
        }

        private void FlowListView_OnFlowItemTapped(object sender, ItemTappedEventArgs e)
        {
        }
    }
}