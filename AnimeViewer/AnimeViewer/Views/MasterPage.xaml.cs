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
            if (Device.OS == TargetPlatform.iOS)
                Icon = "menu.png";
            InitializeComponent();
        }

        public INavigation GlobalNavigation
        {
            get
            {
                return _globalNavigation ??
                       (_globalNavigation = ((App) Application.Current).DiContainer.Resolve<INavigation>());
            }
            set { _globalNavigation = value; }
        }

        private async void MenuItem_OnTapped(object sender, EventArgs e)
        {
            var menuItem = (MenuItem) sender;
            switch (menuItem.Text)
            {
                case "List":
                    ListMenuItem_OnTapped(menuItem, e);
                    break;
                case "About":
                    AboutMenuItem_OnTapped(menuItem, e);
                    break;
                default:
                    await UserDialogs.Instance.AlertAsync("This feature is still under construction! Please show the developer some love so he finishes this.", "Oops! Pretend you saw nothing");
                    break;
            }

            ((MainPage) Application.Current.MainPage).IsPresented = false;
        }

        private void MakeAllMenuItemsInactive()
        {
            foreach (var view in MenuItemsContainer.Children)
            {
                var item = (MenuItem) view;
                item.IsActive = false;
            }
        }

        private void ListMenuItem_OnTapped(MenuItem menuItem, EventArgs e)
        {
            MakeAllMenuItemsInactive();
            menuItem.IsActive = true;
        }

        private async void AboutMenuItem_OnTapped(MenuItem menuItem, EventArgs e)
        {
            await GlobalNavigation.PushAsync(new AboutPage());
        }
    }
}