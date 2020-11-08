using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Common.Utils;

namespace SoundMixerSoftware.Models
{
    public class SystemThemeModel : ThemeModel, INotifyPropertyChanged
    {
        #region Constant

        public const string NAME = "System Theme";
        
        #endregion
        
        #region Constructor

        public SystemThemeModel() : base(new SolidColorBrush(ThemeManager.ImmersiveTheme), NAME)
        {
            ThemeManager.ThemeWrapper.ThemeChanged += (sender, args) =>
            {
                PrimaryColor = CreateBrush(args.Color);
                OnPropertyChanged(nameof(PrimaryColor));        
            };
        }
        
        #endregion
        
        #region Private Methods
        
        private SolidColorBrush CreateBrush(int argb) => new SolidColorBrush(ColorUtil.FromArgb(argb));
        
        #endregion
        
        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}