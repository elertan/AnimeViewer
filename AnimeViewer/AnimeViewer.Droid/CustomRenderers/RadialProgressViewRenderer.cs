using System;
using System.ComponentModel;
using AnimeViewer.Controls;
using AnimeViewer.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RadialProgressView), typeof(RadialProgressViewRenderer))]

namespace AnimeViewer.Droid.CustomRenderers
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

                var progressView = new RadialProgress.RadialProgressView(Forms.Context);

                // Set given properties
                if (_model.ValueToLabelTextFunc != null) progressView.LabelTextDelegate = _model.ValueToLabelTextFunc;
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
                Control.LabelTextDelegate = _model.ValueToLabelTextFunc;
        }
    }
}