using System;
using Xamarin.Forms;

namespace AnimeViewer.Controls
{
    public class RadialProgressView : View
    {
        public static readonly BindableProperty ValueToLabelTextFuncProperty = BindableProperty.Create("ValueToLabelTextFuncProperty", typeof(Func<float, string>), typeof(RadialProgressView), default(Type));
        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create("MaxValueProperty", typeof(float), typeof(RadialProgressView), default(float));
        public static readonly BindableProperty MinValueProperty = BindableProperty.Create("MinValueProperty", typeof(float), typeof(RadialProgressView), default(float));
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("ValueProperty", typeof(float), typeof(RadialProgressView), default(float));

        public Func<float, string> ValueToLabelTextFunc
        {
            get { return (Func<float, string>) GetValue(ValueToLabelTextFuncProperty); }
            set { SetValue(ValueToLabelTextFuncProperty, value); }
        }

        public float MaxValue
        {
            get { return (float) GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public float MinValue
        {
            get { return (float) GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public float Value
        {
            get { return (float) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}