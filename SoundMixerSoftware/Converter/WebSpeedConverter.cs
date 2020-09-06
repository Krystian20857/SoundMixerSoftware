using System;
using System.Globalization;
using System.Windows.Data;
using SoundMixerSoftware.Common.Web;

namespace SoundMixerSoftware.Converter
{
    [ValueConversion(typeof(double), typeof(string))]
    public class WebSpeedConverter : BaseConverter, IValueConverter
    {
        private SpeedConverter _speedConverter = new SpeedConverter();
    
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is double valueDouble))
                throw new ArgumentException($"Input type have to be {typeof(double).FullName}");
            return _speedConverter.FormatSpeed(valueDouble);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is string valueString))
                throw new ArgumentException($"Input type have to be {typeof(string).FullName}");
            return _speedConverter.GetFromFormatted(valueString);
        }
    }
}