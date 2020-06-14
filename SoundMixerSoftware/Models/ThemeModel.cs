using System.Windows.Media;

namespace SoundMixerSoftware.Models
{
    public class ThemeModel
    {
        #region Properties

        public SolidColorBrush PrimaryColor { get; set; }
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