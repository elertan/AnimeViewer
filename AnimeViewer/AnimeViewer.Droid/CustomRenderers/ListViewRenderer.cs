using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace AnimeViewer.Droid.CustomRenderers
{
    public class ListViewRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var listView = Control;
                listView.NestedScrollingEnabled = true;
            }
        }
    }
}