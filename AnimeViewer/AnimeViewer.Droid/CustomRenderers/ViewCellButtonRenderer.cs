using AnimeViewer.Controls;
using AnimeViewer.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ViewCellButton), typeof(ViewCellButtonRenderer))]

namespace AnimeViewer.Droid.CustomRenderers
{
    public class ViewCellButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;
            // Allow button to be clicked in a viewcell
            Control.Focusable = false;
        }
    }
}