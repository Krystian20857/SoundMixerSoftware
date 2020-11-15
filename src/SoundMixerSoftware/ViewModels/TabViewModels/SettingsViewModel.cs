using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Caliburn.Micro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using NLog;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.LocalSystem;
using SoundMixerSoftware.Framework.Overlay;
using SoundMixerSoftware.Interop.Wrapper;
using SoundMixerSoftware.Models;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// Settings tab view model.
    /// </summary>
    public class SettingsViewModel : ITabModel, INotifyPropertyChanged
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
        private bool _autoUpdate;
        private bool _isDarkThemeChecked;

        private ThemeModel _selectedTheme;
        private BindableCollection<ThemeModel> _themes = new BindableCollection<ThemeModel>();
        
        
        private AutoRunHandle _autoRunHandle = new AutoRunHandle(Assembly.GetExecutingAssembly().Location);
        private DebounceDispatcher _debounceDispatcher = new DebounceDispatcher();

        private IWindowManager _windowManager = new WindowManager();

        #endregion
        
        #region Public Properties

        public static SettingsViewModel Instance => IoC.Get<SettingsViewModel>();

        public bool AutoRun
        {
            get => _autoRunHandle.CheckInstance(false);
            set
            {
                if (value)
                    _autoRunHandle.SetStartup();
                else
                    _autoRunHandle.RemoveStartup();
                _autoRun = value;
                OnPropertyChanged(nameof(AutoRun));
            }
        }

        [SettingPropertyAttribute]
        public bool EnableNotify
        {
            get => _enableNotify;
            set
            {
                ConfigHandler.ConfigStruct.Notification.EnableNotifications = value;
                _enableNotify = value;
                OnPropertyChanged(nameof(EnableNotify));
            }
        }

        [SettingPropertyAttribute]
        public bool EnableOverlay
        {
            get => _enableOverlay;
            set
            {
                ConfigHandler.ConfigStruct.Overlay.EnableOverlay = value;
                _enableOverlay = value;
                OnPropertyChanged(nameof(EnableOverlay));
            }
        }

        [SettingPropertyAttribute(true)]
        public int OverlayFadeTime
        {
            get => _overlayFadeTime;
            set
            {
                ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime = TimeSpan.FromMilliseconds(value);
                OverlayHandler.SetFadeTime((int)ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime.TotalMilliseconds);
                _overlayFadeTime = value;
                OnPropertyChanged(nameof(OverlayFadeTime));
            }
        }
        
        [SettingPropertyAttribute(true)]
        public int NotificationShowTime
        {
            get => _notificationShowTime;
            set
            {
                ConfigHandler.ConfigStruct.Notification.NotificationShowTime = TimeSpan.FromMilliseconds(value);
                _notificationShowTime = value;
                OnPropertyChanged(nameof(NotificationShowTime));
            }
        }

        [SettingPropertyAttribute]
        public bool HideOnStartup
        {
            get => _hideOnStartup;
            set
            {
                ConfigHandler.ConfigStruct.Application.HideOnStartup = value;
                _hideOnStartup = value;
                OnPropertyChanged(nameof(HideOnStartup));
            }
        }

        [SettingPropertyAttribute]
        public ThemeModel SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                ConfigHandler.ConfigStruct.Application.ThemeName = value.ThemeName;
                if (value is SystemThemeModel)
                {
                    ThemeManager.UseImmersiveTheme = true;
                }
                else
                {
                    ThemeManager.UseImmersiveTheme = false;
                    ThemeManager.SetTheme(value.ThemeName);
                }
                OnPropertyChanged(nameof(SelectedTheme));
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

        [SettingPropertyAttribute]
        public bool IsDarkThemeChecked
        {
            get => _isDarkThemeChecked;
            set
            {
                _isDarkThemeChecked = value;
                ThemeManager.UseDarkTheme = value;
                ConfigHandler.ConfigStruct.Application.UseDarkTheme = value;
                OnPropertyChanged(nameof(IsDarkThemeChecked));
            }
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
            OverlayFadeTime = (int)ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime.TotalMilliseconds;
            NotificationShowTime = (int)ConfigHandler.ConfigStruct.Notification.NotificationShowTime.TotalMilliseconds;
            HideOnStartup = ConfigHandler.ConfigStruct.Application.HideOnStartup;
            IsDarkThemeChecked = ConfigHandler.ConfigStruct.Application.UseDarkTheme;

            LoadThemes();
            
            LockConfig = false;
        }

        #endregion
        
        #region Private Events

        public void LogsFolderOpenClick()
        {
            AppUtil.OpenExplorer(LocalContainer.LogsFolder);
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

        #endregion
        
        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
            var property = typeof(SettingsViewModel).GetProperty(propertyName ?? string.Empty);
            if (property != null && !LockConfig)
            {
                var attribute = (SettingPropertyAttribute) Attribute.GetCustomAttribute(property, typeof(SettingPropertyAttribute));
                if (!LockConfig || !attribute.IgnoreConfigLock)
                {
                    if (attribute.Debounce)
                        _debounceDispatcher.Debounce(300, param => ConfigHandler.SaveConfig());
                    else
                        ConfigHandler.SaveConfig();
                }
            }
        }
        
        #endregion
    }

    public class SettingPropertyAttribute : Attribute
    {
        public bool Debounce { get; set; }
        public bool IgnoreConfigLock { get; set; }

        public SettingPropertyAttribute(bool debounce = false, bool ignoreConfigLock = false)
        {
            Debounce = debounce;
            IgnoreConfigLock = ignoreConfigLock;
        }
    }
}