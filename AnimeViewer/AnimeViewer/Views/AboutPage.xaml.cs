using System;
using Xamarin.Forms;

namespace AnimeViewer.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(true);
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            //const float seconds = 5;
            const float updatesPerSecond = 40;
            ProgressView.ValueToLabelTextFunc = val => $"{Math.Ceiling(val)}";

            var lastDateTime = DateTime.Now;
            Device.StartTimer(TimeSpan.FromMilliseconds(1000/updatesPerSecond), () =>
            {
                var value = (DateTime.Now - lastDateTime).Milliseconds/1000f;
                if (ProgressView.Value - value < 0) return false;

                ProgressView.Value -= value;

                lastDateTime = DateTime.Now;
                return true;
            });
        }
    }
}