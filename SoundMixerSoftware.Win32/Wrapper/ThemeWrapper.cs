using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SoundMixerSoftware.Win32.Interop.Method;

namespace SoundMixerSoftware.Win32.Wrapper
{
    public class ThemeWrapper : IDisposable
    {
        #region Const
        
        private const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x320;
        private const int WM_DWMCOMPOSITIONCHANGED = 0x31E;
        private const int WM_THEMECHANGED = 0x031A;
        
        #endregion
        
        #region Private Fields
        
        private NativeWindowWrapper _windowWrapper = new NativeWindowWrapper();
        
        #endregion
        
        #region Public Events

        /// <summary>
        /// Fires when theme color has changed.
        /// </summary>
        public event EventHandler<ThemeColorChangedArgs> ThemeChanged;
        
        #endregion

        #region Constructor

        public ThemeWrapper()
        {
            _windowWrapper.MessageReceived += OnMessageReceived;
        }
        
        #endregion
        
        #region Private Events

        private void OnMessageReceived(object sender, Message e)
        {
            var message = e.Msg;
            switch (message)
            {
                case WM_DWMCOLORIZATIONCOLORCHANGED:
                case WM_DWMCOMPOSITIONCHANGED:
                case WM_THEMECHANGED:
                    ThemeChanged?.Invoke(this, new ThemeColorChangedArgs(GetThemeColor()));
                    break;
            }
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Get system color theme as unsigned integer.
        /// </summary>
        /// <returns>ARGB unsigned integer.</returns>
        public int GetThemeColor()
        {
            var color = UxTheme.GetImmersiveColorFromColorSetEx(
                (uint)UxTheme.GetImmersiveUserColorSetPreference(false, false),
                UxTheme.GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")),
                false, 0);
            return (int)((color & 0xFF000000) | ((color & 0x000000FF) << 16) | (color & 0x0000FF00) | ((color & 0x00FF0000) >> 16));
        }
        
        #endregion

        public void Dispose()
        {
            _windowWrapper.DestroyHandle();
        }
    }

    public class ThemeColorChangedArgs : EventArgs
    {
        /// <summary>
        /// System color theme.
        /// </summary>
        public int Color { get; set; }

        public ThemeColorChangedArgs(int color)
        {
            Color = color;
        }
    }
}