using System;
using System.CodeDom;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SoundMixerSoftware.Helpers.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType != typeof(Visibility))
                throw new InvalidOperationException("Type mus be Visibility.");
            var invert = parameter == null ? false : bool.TryParse(parameter as string, out var result) ? result : false;
            if (invert)
                return !(value as bool? ?? false) ? Visibility.Visible : Visibility.Collapsed;
            return value as bool? ?? false ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}