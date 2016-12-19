using System;
using Xamarin.Forms;

namespace AnimeViewer.Views.Partials
{
    public partial class MenuItem : ContentView
    {
        private ImageSource _image;

        private bool _isActive;
        private string _text;

        public MenuItem()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
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

        public ImageSource Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler Tapped;

        private void OnTapped()
        {
            Tapped?.Invoke(this, EventArgs.Empty);
        }

        private void MenuItem_OnTapped(object sender, EventArgs e)
        {
            OnTapped();
        }
    }
}