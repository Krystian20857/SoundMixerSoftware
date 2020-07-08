using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Handles global theme.
    /// </summary>
    public static class ThemeManager
    {
        #region Fields
        
        private static readonly PaletteHelper _paletteHelper = new PaletteHelper();

        #endregion
        
        #region Properties

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Get current theme
        /// </summary>
        /// <returns></returns>
        public static ITheme GetTheme()
        {
            return _paletteHelper.GetTheme();
        }

        /// <summary>
        /// Set theme to specified one.
        /// </summary>
        /// <param name="themeName"></param>
        public static void SetTheme(string themeName)
        {
            var swatch = SwatchHelper.Swatches.FirstOrDefault(x => x.Name.Equals(themeName)) ?? SwatchHelper.Swatches.First();
            var count = swatch.Hues.Count();
            var hues = swatch.Hues.ToList();
            var primaryColor = hues[count - 1];
            var secondaryColor = hues[count - 2];
            _paletteHelper.SetTheme(Theme.Create(Theme.Light, primaryColor, secondaryColor));
        }
        
        #endregion
        
    }
}