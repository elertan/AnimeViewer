using System;
using Xamarin.Forms;

namespace AnimeViewer.Controls
{
    public class GradientContentView : ContentView
    {
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create("StartColorProperty", typeof(Color), typeof(GradientContentView), default(Color));
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create("EndColor", typeof(Color), typeof(GradientContentView), default(Color));

        public Color EndColor
        {
            get { return (Color) GetValue(EndColorProperty); }
            set { SetValue(EndColorProperty, value); }
        }

        public Color StartColor
        {
            get { return (Color) GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }
    }
}