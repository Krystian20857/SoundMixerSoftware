using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// Settings tab view model.
    /// </summary>
    public class SettingsViewModel : ITabModel
    {
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion

        #region Constructor
        
        public SettingsViewModel()
        {
            Name = "Settings";
            Icon = PackIconKind.Cogs;
        }
        
        #endregion
    }
}