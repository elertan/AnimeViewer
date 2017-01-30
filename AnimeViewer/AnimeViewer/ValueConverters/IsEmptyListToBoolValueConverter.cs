using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace AnimeViewer.ValueConverters
{
    public class IsEmptyListToBoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return true;
            var enumerable = (IList) value;
            return enumerable.Count == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}