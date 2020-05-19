using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace SoundMixerSoftware.Helpers.Converters
{
    public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter where T: class, new()
    {
        #region Private Staitc Fields
        
        private static T _converter = null;
    
        #endregion
        
        #region Constructor
        
        public ConverterMarkupExtension()
        {
        }
        
        #endregion
    
        #region Overriden Methods
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new T());
        }
        
        #endregion
        
        #region Abstract Methods
    
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
        
        #endregion
    }
}