using System.Linq;
using System.Runtime;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Interop.Wrapper;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Handles global theme.
    /// </summary>
    public static class ThemeManager
    {

        #region Fields
        
        private static readonly PaletteHelper _paletteHelper = new PaletteHelper();

        private static bool _useImmersiveTheme;

        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Get theme wrapper.
        /// </summary>
        public static ThemeWrapper ThemeWrapper { get; } = new ThemeWrapper();

        /// <summary>
        /// Get current theme
        /// </summary>
        public static ITheme CurrentTheme => _paletteHelper.GetTheme();

        /// <summary>
        /// Use theme associated with os.
        /// </summary>
        public static bool UseImmersiveTheme
        {
            get => _useImmersiveTheme;
            set
            {
                _useImmersiveTheme = value;
                if(value)
                    SetTheme(ImmersiveTheme);
            }
        }

        /// <summary>
        /// Get system theme.
        /// </summary>
        public static Color ImmersiveTheme => ColorUtil.FromArgb(ThemeWrapper.GetThemeColor());

        public static bool UseDarkTheme
        {
            set
            {
                var theme = _paletteHelper.GetTheme();
                
                if(value)
                    theme.SetBaseTheme(Theme.Dark);
                else
                    theme.SetBaseTheme(Theme.Light);
                
                _paletteHelper.SetTheme(theme);
            }
            get
            {
                if (_useImmersiveTheme)
                    return false;

                return _paletteHelper.GetTheme().GetBaseTheme() == BaseTheme.Dark;
            }
        }

        #endregion
        
        #region Constructor

        static ThemeManager()
        {
            ThemeWrapper.ThemeChanged += (sender, args) =>
            {
                if(UseImmersiveTheme)
                    SetTheme(ColorUtil.FromArgb(args.Color));
            };
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Set theme to specified one.
        /// </summary>
        /// <param name="themeName"></param>
        public static void SetTheme(string themeName)
        {
            var theme = _paletteHelper.GetTheme();
            
            var swatch = SwatchHelper.Swatches.FirstOrDefault(x => x.Name.Equals(themeName)) ?? SwatchHelper.Swatches.First();
            var hues = swatch.Hues.ToList();
            var count = hues.Count;
            
            theme.SetPrimaryColor(hues[count - 1]);
            theme.SetSecondaryColor(hues[count - 2]);
            
            _paletteHelper.SetTheme(theme);
        }

        /// <summary>
        /// Set theme to specified color.
        /// </summary>
        /// <param name="color"></param>
        public static void SetTheme(Color color)
        {
            var theme = _paletteHelper.GetTheme();
            
            theme.SetPrimaryColor(color);
            theme.SetSecondaryColor(color.Lighten());

            _paletteHelper.SetTheme(theme);   
        }
        
        #endregion
        
    }
}