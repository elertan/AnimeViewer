using System;
using System.Globalization;
using Xamarin.Forms;

namespace AnimeViewer.ValueConverters
{
    public class IntToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = (int) value;
            return num.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string) value;
            try
            {
                return string.IsNullOrWhiteSpace(str) ? 0 : System.Convert.ToInt32(str);
            }
            catch (OverflowException ex)
            {
                //
                return Int32.MaxValue;
            }
        }
    }
}