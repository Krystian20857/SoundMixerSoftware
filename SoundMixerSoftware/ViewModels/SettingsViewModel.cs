using System.Reflection;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// Settings tab view model.
    /// </summary>
    public class SettingsViewModel : ITabModel
    {
        #region Private Fields
        
        private bool _autoRun;
        
        private AutoRunHandle _autoRunHandle = new AutoRunHandle(Assembly.GetExecutingAssembly().Location);
        
        #endregion
        
        #region Public Properties

        public bool AutoRun
        {
            get => _autoRun;
            set
            {
                if (value)
                    _autoRunHandle.SetStartup();
                else
                    _autoRunHandle.RemoveStartup();
                _autoRun = value;
            }
        }

        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion

        #region Constructor
        
        public SettingsViewModel()
        {
            Name = "Settings";
            Icon = PackIconKind.Cogs;

            AutoRun = _autoRunHandle.CheckInstance();
        }
        
        #endregion
    }
}