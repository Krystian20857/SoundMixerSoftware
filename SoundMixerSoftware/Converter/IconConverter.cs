using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace SoundMixerSoftware.Converter
{
    /// <summary>
    /// Converts boolean to two icons.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(PackIconKind))]
    public class IconConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(parameter is Enum[]))
                throw new InvalidCastException("Parameter must me enum array.");
            var icons = parameter as Enum[];
            return new PackIcon {Kind = (bool)value ? (PackIconKind)icons[0] : (PackIconKind)icons[1]};
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}