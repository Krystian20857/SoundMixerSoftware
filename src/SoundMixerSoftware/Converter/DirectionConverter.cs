using System;
using System.Globalization;
using System.Windows.Data;

namespace SoundMixerSoftware.Converter
{
    public class DirectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter == null || !(parameter is Type[] type))
                return default;
            if (type.Length < 2)
                return default;
            var valueCount = values.Length;
            if (valueCount < 1 || valueCount > 2)
                return default;
            var value = values[1];
            if (!(value is IConvertible convertible))
                return default;
            return convertible.ToType(type[1], null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if(parameter == null || !(parameter is Type[] type))
                return default;
            if (type.Length == 0)
                return default;
            if(!(value is IConvertible convertible))
                return default;
            var intValue = convertible.ToType(type[0], null);
            var typeCount = targetTypes.Length;
            if (typeCount >= 1 && typeCount <= 2)
                return new[] {intValue};
            return default;
        }
    }
}