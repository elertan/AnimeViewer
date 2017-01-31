using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AnimeViewer.EventArguments;
using AnimeViewer.Models;
using MvvmHelpers;
using Xamarin.Forms;

namespace AnimeViewer.ViewModels
{
    public class AdvancedSearchOptionsViewModel : BaseViewModel
    {
        private bool _languageShowAllLanguages = true;
        private bool _languageShowDubbedLanguage;
        private bool _languageShowSubbedLanguage;
        private bool _mainOnlyShowFavoritedAnimes;
        private bool _mainOnlyShowPopularAnimes;

        public AdvancedSearchOptionsViewModel()
        {
            RestoreDefaultsCommand = new Command(RestoreDefaultFilter);
        }

        public ICommand RestoreDefaultsCommand { get; set; }

        public bool MainOnlyShowFavoritedAnimes
        {
            get { return _mainOnlyShowFavoritedAnimes; }
            set
            {
                _mainOnlyShowFavoritedAnimes = value;
                OnPropertyChanged();
            }
        }

        public bool MainOnlyShowPopularAnimes
        {
            get { return _mainOnlyShowPopularAnimes; }
            set
            {
                _mainOnlyShowPopularAnimes = value;
                OnPropertyChanged();
            }
        }

        public bool LanguageShowAllLanguages
        {
            get { return _languageShowAllLanguages; }
            set
            {
                _languageShowAllLanguages = value;
                if (value && LanguageShowDubbedLanguage) LanguageShowDubbedLanguage = false;
                if (value && LanguageShowSubbedLanguage) LanguageShowSubbedLanguage = false;
                OnPropertyChanged();
            }
        }

        public bool LanguageShowDubbedLanguage
        {
            get { return _languageShowDubbedLanguage; }
            set
            {
                _languageShowDubbedLanguage = value;
                if (value && LanguageShowAllLanguages) LanguageShowAllLanguages = false;
                if (!value && !LanguageShowSubbedLanguage) LanguageShowAllLanguages = true;
                OnPropertyChanged();
            }
        }

        public bool LanguageShowSubbedLanguage
        {
            get { return _languageShowSubbedLanguage; }
            set
            {
                _languageShowSubbedLanguage = value;
                if (value && LanguageShowAllLanguages) LanguageShowAllLanguages = false;
                if (!value && !LanguageShowDubbedLanguage) LanguageShowAllLanguages = true;
                OnPropertyChanged();
            }
        }

        public event EventHandler<AnimesFilteredEventArgs> AnimesFiltered;

        public void RestoreDefaultFilter()
        {
            MainOnlyShowFavoritedAnimes = false;
            LanguageShowAllLanguages = true;
        }

        /// <summary>
        ///     Filters the list of animes given with the set of filters
        /// </summary>
        /// <param name="toBeFilteredAnimesanimes">The to be filtered animes</param>
        /// <returns></returns>
        public async Task<IEnumerable<Anime>> ApplyFilterAsync(IEnumerable<Anime> toBeFilteredAnimesanimes)
        {
            var filterMethods = new List<Func<IEnumerable<Anime>, Task<IEnumerable<Anime>>>>();

            // Main
            if (MainOnlyShowFavoritedAnimes) filterMethods.Add(async animes => await Task.Run(() => animes.Where(anime => anime.IsFavourited)));
            if (MainOnlyShowPopularAnimes)
            {
            }
            //filterMethods.Add(async animes => await Task.Run(async () =>
            //{
            //    var popularAnimes = await AnimeManager.Instance.GetMostPopularAnimesAsync();
            //    return animes.Where(a => { return popularAnimes.FirstOrDefault(pa => (a.Name == pa.Name) && (a.Language == pa.Language)) != null; });
            //}));

            // Language
            // both
            if (LanguageShowSubbedLanguage && LanguageShowDubbedLanguage) filterMethods.Add(async animes => await Task.Run(() => animes.Where(anime => (anime.Language == AnimeLanguage.EnglishDub) || (anime.Language == AnimeLanguage.JapaneseSub))));
            // dubbed
            else if (LanguageShowDubbedLanguage) filterMethods.Add(async animes => await Task.Run(() => animes.Where(anime => anime.Language == AnimeLanguage.EnglishDub)));
            // subbed
            else if (LanguageShowSubbedLanguage) filterMethods.Add(async animes => await Task.Run(() => animes.Where(anime => anime.Language == AnimeLanguage.JapaneseSub)));

            foreach (var filterMethod in filterMethods)
                toBeFilteredAnimesanimes = await filterMethod(toBeFilteredAnimesanimes);
            OnAnimesFiltered(new AnimesFilteredEventArgs {Animes = toBeFilteredAnimesanimes});
            return toBeFilteredAnimesanimes;
        }

        protected virtual void OnAnimesFiltered(AnimesFilteredEventArgs e)
        {
            AnimesFiltered?.Invoke(this, e);
        }
    }
}