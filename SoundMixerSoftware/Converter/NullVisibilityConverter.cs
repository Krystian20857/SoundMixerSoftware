using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SoundMixerSoftware.Converter
{
    [ValueConversion(typeof(string), typeof(VisibilityConverter))]
    public class NullVisibilityConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (value is string stringValue)
                return string.IsNullOrWhiteSpace(stringValue) ? Visibility.Collapsed : Visibility.Visible;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}