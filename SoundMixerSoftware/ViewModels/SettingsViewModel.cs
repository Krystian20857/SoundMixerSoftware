using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;
using NLog;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.LocalSystem;
using SoundMixerSoftware.Helpers.Overlay;
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
        private bool _enableOverlay;
        private int _fadeTime;
        
        private AutoRunHandle _autoRunHandle = new AutoRunHandle(Assembly.GetExecutingAssembly().Location);
        private DebounceDispatcher _debounceDispatcher = new DebounceDispatcher();
        
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

        public bool EnableOverlay
        {
            get => _enableOverlay;
            set
            {
                ConfigHandler.ConfigStruct.EnableOverlay = value;
                if(!LockConfig)
                    ConfigHandler.SaveConfig();
                Logger.Trace($"Changed overlay-enable setting to {value}");
                _enableOverlay = value;
            }
        }

        public int FadeTime
        {
            get => _fadeTime;
            set
            {
                ConfigHandler.ConfigStruct.FadeTime = value;
                if(!LockConfig)
                    _debounceDispatcher.Debounce(300,(param) =>
                    {
                        ConfigHandler.SaveConfig();
                        OverlayHandler.SetFadeTime(value);
                    });
                _fadeTime = value;
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
            EnableNotify = ConfigHandler.ConfigStruct.EnableNotifications;
            EnableOverlay = ConfigHandler.ConfigStruct.EnableOverlay;
            FadeTime = ConfigHandler.ConfigStruct.FadeTime;

            LockConfig = false;
        }
        
        #endregion
        
        #region Private Events

        public void LogsFolderOpenClick()
        {
            AppUtils.OpenExplorer(LocalContainer.LogsFolder);
        }
        
        #endregion
    }
}