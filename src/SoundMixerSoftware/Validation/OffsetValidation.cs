using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SoundMixerSoftware.Validation
{
    public class MaxIntegerValidation : IntegerValidation
    {
        #region Public Properties

        public MaxIntegerValidationWrapper MaxValueWrapper { get; set; }

        #endregion
        
        #region Constructor
        
        public MaxIntegerValidation(){}
        
        #endregion
        
        #region Overriden Methods
        
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = base.Validate(value, cultureInfo);
            if(!result.IsValid)
                return new ValidationResult(false, result.ErrorContent);
            if((int)result.ErrorContent > MaxValueWrapper.MaxValue)
                return new ValidationResult(false, "Value is too big :0.");
            return ValidationResult.ValidResult;
        }
        
        #endregion
    }

    public class MaxIntegerValidationWrapper : DependencyObject
    {
        #region Static
        
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue),
            typeof(int),
            typeof(MaxIntegerValidationWrapper),
            new FrameworkPropertyMetadata(0));
        
        #endregion
        
        #region Properties

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        #endregion
    }
}