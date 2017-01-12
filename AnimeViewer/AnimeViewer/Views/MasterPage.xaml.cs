using System;
using Acr.UserDialogs;
using Microsoft.Practices.Unity;
using Xamarin.Forms;
using MenuItem = AnimeViewer.Views.Partials.MenuItem;

// ReSharper disable SwitchStatementMissingSomeCases

namespace AnimeViewer.Views
{
    public partial class MasterPage
    {
        private INavigation _globalNavigation;

        public MasterPage()
        {
            // If our device is running iOS, apply an icon to our masterpage (the hamburger icon, since by default iOS doesnt include one and android does)
            if (Device.OS == TargetPlatform.iOS)
                Icon = "menu.png";
            InitializeComponent();
        }

        /// <summary>
        ///     This navigation is globally accessible and is linked to our detailpage's navigationpage, retrieved via our losely
        ///     coupled container to easily change our main navigation
        /// </summary>
        public INavigation GlobalNavigation
        {
            get
            {
                return _globalNavigation ??
                       (_globalNavigation = ((App) Application.Current).DiContainer.Resolve<INavigation>());
            }
            set { _globalNavigation = value; }
        }

        /// <summary>
        ///     This method is linked via our view and gets called once a menu item gets tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_OnTapped(object sender, EventArgs e)
        {
            // Get our menu item via the sender
            var menuItem = (MenuItem) sender;
            // Check our items text and do things depending on which item it is
            switch (menuItem.Text)
            {
                case "List":
                    ListMenuItem_OnTapped(menuItem, e);
                    break;
                case "About":
                    AboutMenuItem_OnTapped(menuItem, e);
                    break;
                case "Newest":
                    NewestMenuItem_OnTapped(menuItem, e);
                    break;
                default:
                    await
                        UserDialogs.Instance.AlertAsync(
                            "This feature is still under construction! Please show the developer some love so he finishes this.",
                            "Oops! Pretend you saw nothing");
                    break;
            }

            ((MainPage) Application.Current.MainPage).IsPresented = false;
        }

        /// <summary>
        ///     Makes all menu items inactive (dimmed)
        /// </summary>
        private void MakeAllMenuItemsInactive()
        {
            foreach (var view in MenuItemsContainer.Children)
            {
                var item = (MenuItem) view;
                item.IsActive = false;
            }
        }

        private async void NewestMenuItem_OnTapped(MenuItem menuItem, EventArgs e)
        {
            MakeAllMenuItemsInactive();
            menuItem.IsActive = true;
            var animes = await AnimeManager.Instance.GetUpdatedAnimes();
            await GlobalNavigation.PushAsync(new AnimeCollectionPage(animes));
        }

        private void ListMenuItem_OnTapped(MenuItem menuItem, EventArgs e)
        {
            MakeAllMenuItemsInactive();
            menuItem.IsActive = true;
        }

        private async void AboutMenuItem_OnTapped(MenuItem menuItem, EventArgs e)
        {
            // Show our about page
            await GlobalNavigation.PushAsync(new AboutPage());
        }
    }
}