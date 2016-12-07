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
    }
}