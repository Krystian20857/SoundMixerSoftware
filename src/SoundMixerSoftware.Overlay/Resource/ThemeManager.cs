using System;
using GameOverlay.Drawing;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Overlay.Resource
{
    public class ThemeManager
    {
        #region Static Fields
        
        private static ThemeWrapper _themeWrapper = new ThemeWrapper();
        
        #endregion
        
        #region Static Propeties

        public static Color ThemeColor { get; private set; }

        #endregion
        
        #region Events

        public static event EventHandler ReloadResources;
        
        #endregion
        
        #region Static Constructor

        #endregion
        
        #region Static Methods

        public static void Initialize()
        {
            if (SystemVersion.IsWin8OrHigher())
                ThemeColor = Color.FromARGB(_themeWrapper.GetThemeColor());
            else
                ThemeColor = Color.FromARGB(unchecked((int) 0xFF03F0FC));
            _themeWrapper.ThemeChanged += ThemeWrapperOnThemeChanged;
        }

        #endregion
        
        #region Private Events
        
        private static void ThemeWrapperOnThemeChanged(object sender, ThemeColorChangedArgs e)
        {
            ThemeColor = Color.FromARGB(_themeWrapper.GetThemeColor());
            ReloadResources?.Invoke(sender, EventArgs.Empty);
        }
        
        #endregion
    }
}