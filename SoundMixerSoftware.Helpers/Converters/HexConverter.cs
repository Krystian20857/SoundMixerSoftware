using System;
using System.Globalization;
using System.Windows.Data;

namespace SoundMixerSoftware.Helpers.Converters
{
    /// <summary>
    /// Converts hex string to unsigned integer and vice versa.
    /// </summary>
    [ValueConversion(typeof(uint), typeof(string))]
    public class HexConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "0x" + ((uint) value).ToString("X2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hex = value.ToString();
            if (hex.StartsWith("0x"))
                hex = hex.Remove(0, 2);
            return !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result) ? 0 : result;
        }
    }
}