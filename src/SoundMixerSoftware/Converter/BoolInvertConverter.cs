﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace SoundMixerSoftware.Converter
{
    /// <summary>
    /// Inverts bool: ! operator.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolInvertConverter : BaseConverter, IValueConverter
    {
        #region Overriden Methods
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType != typeof(bool))
                throw new InvalidOperationException("type must be bool.");
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType != typeof(bool))
                throw new InvalidOperationException("type must be bool.");
            return !(bool) value;
        }
        
        #endregion
    }
}