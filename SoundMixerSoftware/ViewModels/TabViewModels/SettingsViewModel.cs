using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
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
using SoundMixerSoftware.Framework.NotifyWrapper;
using SoundMixerSoftware.Framework.Overlay;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Updater;
using SoundMixerSoftware.Utils;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Wrapper;
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

        private string _updateStatus;
        private SolidColorBrush _updateStatusColor;

        private bool _autoRun;
        private bool _enableNotify;
        private bool _enableOverlay;
        private bool _hideOnStartup;
        private int _overlayFadeTime;
        private int _notificationShowTime;
        private EnumDisplayModel<UpdateMode> _updateRunMode;
        private bool _autoUpdate;

        private ThemeModel _selectedTheme;
        private BindableCollection<ThemeModel> _themes = new BindableCollection<ThemeModel>();

        private TabModel _selectedTab;
        private BindableCollection<TabModel> _tabs = new BindableCollection<TabModel>();
        
        private AutoRunHandle _autoRunHandle = new AutoRunHandle(Assembly.GetExecutingAssembly().Location);
        private DebounceDispatcher _debounceDispatcher = new DebounceDispatcher();

        private Timer dotdotdotTimer;
        private INotification<NewVersionEventArgs> _updateNotification = new NewVersionNotification();
        
        private IWindowManager _windowManager = new WindowManager();

        #endregion
        
        #region Public Properties

        public static SettingsViewModel Instance => IoC.Get<SettingsViewModel>();

        public string AppVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public Updater.Updater Updater => Bootstrapper.Instance.Updater;

        public bool HasNewVersion => Updater.HasNewVersion;

        public string UpdateStatus
        {
            get => _updateStatus;
            set
            {
                _updateStatus = value;
                OnPropertyChanged(nameof(UpdateStatus));
            }
        }

        public SolidColorBrush UpdateStatusColor
        {
            get => _updateStatusColor;
            set
            {
                _updateStatusColor = value;
                OnPropertyChanged(nameof(UpdateStatusColor));
            }
        }
        
        public BindableCollection<EnumDisplayModel<UpdateMode>> UpdateModes { get; set; } = new BindableCollection<EnumDisplayModel<UpdateMode>>();

        public EnumDisplayModel<UpdateMode> UpdateRunModeEnum
        {
            get => _updateRunMode;
            set
            {
                _updateRunMode = value;
                ConfigHandler.ConfigStruct.Updater.Mode = value.EnumValue;
                Updater.Mode = value.EnumValue;
                if(!LockConfig)
                    ConfigHandler.SaveConfig();
                Logger.Trace($"Changed update mode setting to: {value}");
                OnPropertyChanged(nameof(UpdateRunModeEnum));
            }
        }

        public bool AutoUpdate
        {
            get => _autoUpdate;
            set
            {
                ConfigHandler.ConfigStruct.Updater.AutoUpdate = value;
                if(!LockConfig)
                    ConfigHandler.SaveConfig();
                Logger.Trace($"Changed enable-autoupdate setting to: {value}");
                _autoUpdate = value;
            }
        }

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
            AutoUpdate = ConfigHandler.ConfigStruct.Updater.AutoUpdate;

            LoadThemes();
            LoadTabs();
            LoadUpdater();
            
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

        private void LoadUpdater()
        {
            EnumDisplayHelper.AddItems(UpdateModes);
            UpdateRunModeEnum = UpdateModes.FirstOrDefault(x => x.EnumValue == ConfigHandler.ConfigStruct.Updater.Mode);

            Updater.NewVersionAvailable += (sender, args) =>
            {
                DisposeDotDotDotTimer();
                Execute.OnUIThread(() =>
                {
                    UpdateStatus = $"New version available: v{args.Release.ReleaseVersion}.";
                    UpdateStatusColor = new SolidColorBrush(Colors.LimeGreen);
                    OnPropertyChanged(nameof(HasNewVersion));
                });
                var autoUpdate = ConfigHandler.ConfigStruct.Updater.AutoUpdate;
                if (autoUpdate)
                {
                    Updater.DownloadUpdate();
                }
                if (ConfigHandler.ConfigStruct.Notification.EnableNotifications && Bootstrapper.Instance.MainWindowHandle != User32.GetForegroundWindow() && !autoUpdate)
                {
                    _updateNotification.SetValue(NewVersionNotification.VERSION_KEY, args);
                    _updateNotification.Show();
                }
            };

            Updater.NotNewVersionAvailable += (sender, args) =>
            {
                DisposeDotDotDotTimer();
                Execute.OnUIThread(() =>
                {
                    UpdateStatus = $"You are running newest version!";
                    UpdateStatusColor = new SolidColorBrush(Colors.LimeGreen);
                    OnPropertyChanged(nameof(HasNewVersion));
                });
            };

            Updater.UpdateCheckError += (sender, exception) =>
            {
                DisposeDotDotDotTimer();
                Execute.OnUIThread(() =>
                {
                    UpdateStatus = $"Error while checking new version.";
                    UpdateStatusColor = new SolidColorBrush(Colors.IndianRed);
                    OnPropertyChanged(nameof(HasNewVersion));
                });
            };

            Updater.FileDownloaded += (sender, args) =>
            {
                if (ConfigHandler.ConfigStruct.Updater.AutoUpdate)
                {
                    Updater.RunInstaller();
                }
            };

            _updateNotification.Clicked += InstallUpdateClick;
            Updater.CheckForUpdate();
        }

        private void DisposeDotDotDotTimer()
        {
            if (dotdotdotTimer == null) return;
            dotdotdotTimer?.Dispose();
            dotdotdotTimer = null;
        }

        public void CheckForUpdateClick()
        {
           _debounceDispatcher.Debounce(50, obj =>
           {
               Updater.CheckForUpdate();
               dotdotdotTimer?.Dispose();
               var counter = 1;
               dotdotdotTimer = new Timer(obj1 =>
               {
                   if (counter > 3)
                       counter = 0;
                   var counter1 = counter;
                   Execute.OnUIThread(() =>
                   {
                       UpdateStatus = new string('.', counter1);
                       UpdateStatusColor = new SolidColorBrush(Colors.Black);
                   });
                   ++counter;
                   dotdotdotTimer?.Change(500, 500);
               }, null, 0, 500);
           });
        }

        public void InstallUpdateClick()
        {
            if (!Updater.HasNewVersion)
            {
                return;
            }

            var updateWindow = UpdateViewModel.Instance;
            updateWindow.StartUpdate();

            _windowManager.ShowDialogAsync(updateWindow);
        }
        
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