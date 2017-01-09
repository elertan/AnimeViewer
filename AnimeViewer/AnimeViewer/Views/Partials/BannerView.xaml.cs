using Xamarin.Forms;

namespace AnimeViewer.Views.Partials
{
    public partial class BannerView : ContentView
    {
        private View _customView;
        private string _text;

        public BannerView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public View CustomView
        {
            get { return _customView; }
            set
            {
                _customView = value;
                OnPropertyChanged();
            }
        }
    }
}