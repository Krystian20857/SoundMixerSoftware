using System.Reflection;
using MaterialDesignThemes.Wpf;
using NLog;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// Settings tab view model.
    /// </summary>
    public class SettingsViewModel : ITabModel
    {
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields
        
        private bool _autoRun;
        private bool _enableNotify;
        
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
                Logger.Trace($"Changed auto-run setting to: {value}");
                _autoRun = value;
            }
        }

        public bool EnableNotify
        {
            get => _enableNotify;
            set
            {
                ConfigHandler.ConfigStruct.EnableNotifications = value;
                if(!LockConfig)
                    ConfigHandler.SaveConfig();
                Logger.Trace($"Changed enable-notification setting to: {value}");
                _enableNotify = value;
            }
        }

        public bool LockConfig { get; set; }

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
            
            LockConfig = true;
            
            AutoRun = _autoRunHandle.CheckInstance();
            EnableNotify = ConfigHandler.ConfigStruct.EnableNotifications ?? false;

            LockConfig = false;
        }
        
        #endregion
    }
}