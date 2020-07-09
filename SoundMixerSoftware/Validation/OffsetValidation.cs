using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SoundMixerSoftware.Validation
{
    public class OffsetValidation : IntegerValidation
    {
        #region Public Properties

        public OffsetValidationWrapper MaxOffsetWrapper { get; set; }

        #endregion
        
        #region Constructor
        
        public OffsetValidation(){}
        
        #endregion
        
        #region Overriden Methods
        
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = base.Validate(value, cultureInfo);
            if(!result.IsValid)
                return new ValidationResult(false, result.ErrorContent);
            if((int)result.ErrorContent > MaxOffsetWrapper.MaxOffset)
                return new ValidationResult(false, "Offset is too big :0.");
            return ValidationResult.ValidResult;
        }
        
        #endregion
    }

    public class OffsetValidationWrapper : DependencyObject
    {
        #region Static
        
        public static readonly DependencyProperty MaxOffsetProperty = DependencyProperty.Register(nameof(MaxOffset),
            typeof(int),
            typeof(OffsetValidationWrapper),
            new FrameworkPropertyMetadata(0));
        
        #endregion
        
        #region Properties

        public int MaxOffset
        {
            get => (int)GetValue(MaxOffsetProperty);
            set => SetValue(MaxOffsetProperty, value);
        }

        #endregion
    }
}