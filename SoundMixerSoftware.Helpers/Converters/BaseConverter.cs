using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SoundMixerSoftware.Helpers.Converters
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}