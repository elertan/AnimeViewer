using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SearchBarRenderer = AnimeViewer.iOS.CustomRenderers.SearchBarRenderer;

[assembly: ExportRenderer(typeof(SearchBar), typeof(SearchBarRenderer))]

namespace AnimeViewer.iOS.CustomRenderers
{
    public class SearchBarRenderer : Xamarin.Forms.Platform.iOS.SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            Control.KeyboardAppearance = UIKeyboardAppearance.Dark;
        }
    }
}