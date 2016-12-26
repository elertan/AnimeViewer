﻿using System.ComponentModel;
using System.Threading.Tasks;
using AnimeViewer.Models;
using MvvmHelpers;

namespace AnimeViewer.ViewModels
{
    public class AnimePageViewModel : BaseViewModel
    {
        private Anime _anime;
        private bool _hasConnectionIssue;

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

        public bool HasConnectionIssue
        {
            get { return _hasConnectionIssue; }
            set
            {
                _hasConnectionIssue = value;
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