using System;
using System.ComponentModel;
using System.Drawing;
using AnimeViewer.Controls;
using AnimeViewer.iOS.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RadialProgressView), typeof(RadialProgressViewRenderer))]

namespace AnimeViewer.iOS.CustomRenderers
{
    public class RadialProgressViewRenderer : ViewRenderer<RadialProgressView, RadialProgress.RadialProgressView>
    {
        private RadialProgressView _model;

        protected override void OnElementChanged(ElementChangedEventArgs<RadialProgressView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                _model = e.NewElement;

                var progressView = new RadialProgress.RadialProgressView
                {
                    Center = new PointF((float) NativeView.Center.X, (float) NativeView.Center.Y - 100)
                };
                // Set given properties
                if (_model.ValueToLabelTextFunc != null) progressView.LabelTextDelegate = val => _model.ValueToLabelTextFunc((float) val);
                if (Math.Abs(_model.MinValue - progressView.MinValue) > 0.1) progressView.MinValue = _model.MinValue;
                if (Math.Abs(_model.MaxValue - progressView.MaxValue) > 0.1) progressView.MaxValue = _model.MaxValue;
                if (Math.Abs(_model.Value - progressView.Value) > 0.1) progressView.Value = _model.Value;

                SetNativeControl(progressView);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // Update values accordingly
            if (e.PropertyName == RadialProgressView.ValueProperty.PropertyName)
                Control.Value = _model.Value;
            else if (e.PropertyName == RadialProgressView.MinValueProperty.PropertyName)
                Control.MinValue = _model.MinValue;
            else if (e.PropertyName == RadialProgressView.MaxValueProperty.PropertyName)
                Control.MaxValue = _model.MaxValue;
            else if (e.PropertyName == RadialProgressView.ValueToLabelTextFuncProperty.PropertyName)
                Control.LabelTextDelegate = val => _model.ValueToLabelTextFunc((float) val);
            //else if (e.PropertyName == VisualElement.WidthProperty.PropertyName)
            //{
            //    var frame = Control.Frame;
            //    frame.Offset(-(_model.Width/2), 0);
            //    Control.Frame = frame;
            //}
        }
    }
}