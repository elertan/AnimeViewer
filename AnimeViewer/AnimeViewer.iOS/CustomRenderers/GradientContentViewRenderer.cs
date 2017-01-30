using AnimeViewer.Controls;
using AnimeViewer.iOS.CustomRenderers;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GradientContentViewRenderer), typeof(GradientContentView))]

namespace AnimeViewer.iOS.CustomRenderers
{
    public class GradientContentViewRenderer : ViewRenderer<GradientContentView, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GradientContentView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var view = new UIView();
                var gradientLayer = new CAGradientLayer
                {
                    Colors = new[] {e.NewElement.StartColor.ToCGColor(), e.NewElement.EndColor.ToCGColor()},
                    StartPoint = new CGPoint(0, 0),
                    EndPoint = new CGPoint(1, 1)
                };

                view.Layer.AddSublayer(gradientLayer);
                SetNativeControl(view);
            }
        }
    }
}