using System;
using Xamarin.Forms;
using MenuItem = AnimeViewer.Views.Partials.MenuItem;

// ReSharper disable SwitchStatementMissingSomeCases

namespace AnimeViewer.Views
{
    public partial class MasterPage : ContentPage
    {
        public MasterPage()
        {
            if (Device.OS == TargetPlatform.iOS)
                Icon = "menu.png";
            InitializeComponent();
        }

        public new INavigation Navigation => ((App) Application.Current).Navigation;

        private void MenuItem_OnTapped(object sender, EventArgs e)
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
            await Navigation.PushModalAsync(new AboutPage());
        }
    }
}