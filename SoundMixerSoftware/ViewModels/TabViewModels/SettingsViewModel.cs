using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Caliburn.Micro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using NLog;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.LocalSystem;
using SoundMixerSoftware.Helpers.Overlay;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Views;
using SoundMixerSoftware.Win32.Wrapper;
using LogManager = NLog.LogManager;

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
        private bool _hideOnStartup;
        private int _overlayFadeTime;
        private int _notificationShowTime;

        private ThemeModel _selectedTheme;
        private BindableCollection<ThemeModel> _themes = new BindableCollection<ThemeModel>();

        private TabModel _selectedTab;
        private BindableCollection<TabModel> _tabs = new BindableCollection<TabModel>();
        
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
                ConfigHandler.ConfigStruct.Notification.EnableNotifications = value;
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
                ConfigHandler.ConfigStruct.Overlay.EnableOverlay = value;
                if(!LockConfig)
                    ConfigHandler.SaveConfig();
                Logger.Trace($"Changed overlay-enable setting to {value}");
                _enableOverlay = value;
            }
        }

        public int OverlayFadeTime
        {
            get => _overlayFadeTime;
            set
            {
                ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime = value;
                if(!LockConfig)
                    _debounceDispatcher.Debounce(300,param =>
                    {
                        ConfigHandler.SaveConfig();
                        OverlayHandler.SetFadeTime(value);
                    });
                _overlayFadeTime = value;
            }
        }
        
        public int NotificationShowTime
        {
            get => _notificationShowTime;
            set
            {
                ConfigHandler.ConfigStruct.Notification.NotificationShowTime = value;
                if(!LockConfig)
                    _debounceDispatcher.Debounce(300,param => ConfigHandler.SaveConfig());
                _notificationShowTime = value;
            }
        }

        public bool HideOnStartup
        {
            get => _hideOnStartup;
            set
            {
                ConfigHandler.ConfigStruct.Application.HideOnStartup = value;
                if(!LockConfig)
                    _debounceDispatcher.Debounce(300, param => ConfigHandler.SaveConfig());
                _hideOnStartup = value;
            }
        }

        public ThemeModel SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                ConfigHandler.ConfigStruct.Application.ThemeName = value.ThemeName;
                if(!LockConfig)
                    ConfigHandler.SaveConfig();
                if (value is SystemThemeModel)
                {
                    ThemeManager.UseImmersiveTheme = true;
                }
                else
                {
                    ThemeManager.UseImmersiveTheme = false;
                    ThemeManager.SetTheme(value.ThemeName);
                }
            }
        }

        public BindableCollection<ThemeModel> Themes
        {
            get => _themes;
            set
            {
                _themes = value;
            }
        }

        public TabModel SelectedTab
        {
            get => _selectedTab;
            set
            {
                ConfigHandler.ConfigStruct.Application.SelectedTab = value.Uuid; 
                if(!LockConfig)
                    ConfigHandler.SaveConfig();
                _selectedTab = value;
            }
        }

        public BindableCollection<TabModel> Tabs
        {
            get => _tabs;
            set => _tabs = value;
        }

        public bool LockConfig { get; set; }

        #endregion

        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }
        public Guid Uuid { get; set; } = new Guid("A1504124-4802-404D-8A95-CFE5056CE563");

        #endregion

        #region Constructor

        public SettingsViewModel()
        {
            Name = "Settings";
            Icon = PackIconKind.Cogs;
            
            LockConfig = true;
            
            AutoRun = _autoRunHandle.CheckInstance();
            EnableNotify = ConfigHandler.ConfigStruct.Notification.EnableNotifications;
            EnableOverlay = ConfigHandler.ConfigStruct.Overlay.EnableOverlay;
            OverlayFadeTime = ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime;
            NotificationShowTime = ConfigHandler.ConfigStruct.Notification.NotificationShowTime;
            HideOnStartup = ConfigHandler.ConfigStruct.Application.HideOnStartup;
            
            LoadThemes();
            LoadTabs();
            
            LockConfig = false;
        }

        #endregion
        
        #region Private Events

        public void LogsFolderOpenClick()
        {
            AppUtils.OpenExplorer(LocalContainer.LogsFolder);
        }

        private void LoadThemes()
        {
            if(SystemVersion.IsWin8OrHigher())
                Themes.Add(new SystemThemeModel());
            
            foreach (var theme in  SwatchHelper.Swatches)
                Themes.Add(new ThemeModel(new SolidColorBrush(theme.Hues.Last()), theme.Name));

            var themeConfig = ConfigHandler.ConfigStruct.Application.ThemeName;
            SelectedTheme = Themes.FirstOrDefault(x => x.ThemeName.Equals(themeConfig)) ?? Themes[0];
        }

        private void LoadTabs()
        {
            var mainView = MainViewModel.Instance;

            Tabs.Clear();
            foreach (var tab in mainView.Tabs)
                Tabs.Add(TabModel.CreateModel(tab));

            mainView.Tabs.CollectionChanged += (sender, args) =>
            {
                if(args.NewStartingIndex != -1)
                    foreach (var tab in args.NewItems.Cast<ITabModel>())
                        Tabs.Add(TabModel.CreateModel(tab));
                if(args.OldStartingIndex != -1)
                    foreach (var tab in args.NewItems.Cast<ITabModel>())
                        Tabs.Remove(TabModel.CreateModel(tab));
            };
        }
        
        #endregion
    }
}