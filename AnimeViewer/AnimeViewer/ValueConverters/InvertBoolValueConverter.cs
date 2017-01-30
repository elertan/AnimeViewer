using System;
using System.Globalization;
using Xamarin.Forms;

namespace AnimeViewer.ValueConverters
{
    public class InvertBoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool) value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool) value;
            return !b;
        }
    }
}