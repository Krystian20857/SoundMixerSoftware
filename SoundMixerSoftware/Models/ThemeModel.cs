using System.Windows.Media;

namespace SoundMixerSoftware.Models
{
    public class ThemeModel
    {
        #region Properties

        /// <summary>
        /// Primary color fo theme.
        /// </summary>
        public SolidColorBrush PrimaryColor { get; set; }
        /// <summary>
        /// Theme name.
        /// </summary>
        public string ThemeName { get; set; }

        #endregion
        
        #region Constructor
        
        public ThemeModel(SolidColorBrush primaryColor, string themeName)
        {
            PrimaryColor = primaryColor;
            ThemeName = themeName;
        }
        
        #endregion
    }
}