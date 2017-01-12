using System.Collections.Generic;
using AnimeViewer.Models;
using MvvmHelpers;

namespace AnimeViewer.ViewModels
{
    internal class AnimeCollectionPageViewModel : BaseViewModel
    {
        public IEnumerable<Anime> Animes { get; set; }
    }
}