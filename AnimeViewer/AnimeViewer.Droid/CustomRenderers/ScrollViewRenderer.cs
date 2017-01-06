using Xamarin.Forms.Platform.Android;

namespace AnimeViewer.Droid.CustomRenderers
{
    public class ScrollViewRenderer : Xamarin.Forms.Platform.Android.ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
            }
        }
    }
}