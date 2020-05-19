using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace SoundMixerSoftware.Helpers.Converters
{
    /// <summary>
    /// Converts bool to specified mute icon.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(PackIconKind))]
    public class MuteIconConverter : ConverterMarkupExtension<MuteIconConverter>
    {
        #region Public Static Properties

        /// <summary>
        /// Mute Icon
        /// </summary>
        public static PackIconKind MuteIcon { get; set; } = PackIconKind.VolumeMute;

        /// <summary>
        /// Unmute Icon
        /// </summary>
        public static PackIconKind UnMuteIcon { get; set; } = PackIconKind.VolumeHigh;

        #endregion
        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new PackIcon{Kind = UnMuteIcon} : new PackIcon{Kind = MuteIcon};
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}