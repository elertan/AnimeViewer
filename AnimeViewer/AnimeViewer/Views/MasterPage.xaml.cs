using System;
using Xamarin.Forms;

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

        private void MasterPage_OnAppearing(object sender, EventArgs e)
        {
        }
    }
}