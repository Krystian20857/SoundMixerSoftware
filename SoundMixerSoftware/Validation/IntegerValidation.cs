using System.Globalization;
using System.Windows.Controls;

namespace SoundMixerSoftware.Validation
{
    public class IntegerValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(!(value is string valueString))
                return new ValidationResult(false, "Cannot get text from value.");
            if(!int.TryParse(valueString, out var result))
                return new ValidationResult(false, "Cannot get integer from text.");
            return new ValidationResult(true, result);
        }
    }
}