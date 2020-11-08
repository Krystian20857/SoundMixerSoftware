using System.Linq;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Handles global theme.
    /// </summary>
    public static class ThemeManager
    {
        #region Constant

        private static readonly Color OFFSET_COLOR = Color.FromArgb(0x00, 0x1C, 0x1C, 0x1C);
        
        #endregion
        
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
            var swatch = SwatchHelper.Swatches.FirstOrDefault(x => x.Name.Equals(themeName)) ?? SwatchHelper.Swatches.First();
            var count = swatch.Hues.Count();
            var hues = swatch.Hues.ToList();
            var primaryColor = hues[count - 1];
            var secondaryColor = hues[count - 2];
            _paletteHelper.SetTheme(Theme.Create(Theme.Light, primaryColor, secondaryColor));
        }

        /// <summary>
        /// Set theme to specified color.
        /// </summary>
        /// <param name="color"></param>
        public static void SetTheme(Color color)
        {
            _paletteHelper.SetTheme(Theme.Create(Theme.Light, color, color - OFFSET_COLOR));   
        }
        
        #endregion
        
    }
}