using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using NLog;
using NuGet;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Utils;
using Squirrel;
using LogManager = NLog.LogManager;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace SoundMixerSoftware.ViewModels
{
    public class UpdateViewModel : ITabModel, INotifyPropertyChanged
    {
        #region Fields

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private bool _autoUpdate;
        private string _updateText;
        private int _progressValue;
        private bool _progressBarVisibility;
        private SolidColorBrush _updateTextColor;
        private bool _showRestart;

        private SemanticVersion _lastCheckNonNullVersion;
        private Timer _updateTimer = new Timer(TimeSpan.FromHours(24).TotalMilliseconds);

        #endregion
        
        #region Implemented Properties

        public string Name { get; set; } = "Update";
        public PackIconKind Icon { get; set; } = PackIconKind.Update;
        public Guid Uuid { get; set; } = new Guid("8A5D7580-708A-4AAE-B572-212DF684494E");
        
        #endregion
        
        #region Properties

        public static UpdateViewModel Instance => IoC.Get<UpdateViewModel>();
        public bool LockConfig { get; private set; }
        

        [SettingProperty]
        public bool AutoUpdate
        {
            get => _autoUpdate;
            set
            {
                _autoUpdate = value;
                ConfigHandler.ConfigStruct.Updater.AutoUpdate = value;
                _updateTimer.Enabled = value;
                OnPropertyChanged(nameof(AutoUpdate));
            }
        }

        public string UpdateText
        {
            get => _updateText;
            set
            {
                _updateText = value;
                OnPropertyChanged(nameof(UpdateText));
            }
        }

        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                if (value == 100)
                    ProgressBarVisibility = false;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public bool ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged(nameof(ProgressBarVisibility));
            }
        }

        public SolidColorBrush UpdateTextColor
        {
            get => _updateTextColor;
            set
            {
                _updateTextColor = value;
                OnPropertyChanged(nameof(UpdateTextColor));
            }
        }

        public bool ShowRestart
        {
            get => _showRestart;
            set
            { 
                _showRestart = value;
                OnPropertyChanged(nameof(ShowRestart));
            }
        }

        #endregion
        
        #region Constructor

        public UpdateViewModel()
        {
            LockConfig = true;

            AutoUpdate = ConfigHandler.ConfigStruct.Updater.AutoUpdate;

            LockConfig = false;
            
            CheckForUpdate();

            _updateTimer.AutoReset = true;
            _updateTimer.Elapsed += (sender, args) =>
            {
                CheckForUpdate();
            };
        }
        
        #endregion
        
        #region Methods

        public void CheckForUpdate()
        {
            Task.Factory.StartNew(CheckForUpdateInternal);
        }

        public void RestartClick()
        {
            UpdateManager.RestartApp(arguments: "-forceShow");
        }
        
        private async void CheckForUpdateInternal()
        {
            
            SetStatus("Checking for update...",true, false);
            
            try
            {
                using (var updateManager = await Bootstrapper.Instance.GetUpdateManager())
                {
                    var updateTask = updateManager.UpdateApp(z => SetStatus(progress: z));

                    var currentVersion = Constant.AppVersion.ToSemanticVersion();
                    var release = await updateTask;
                    var isUpdated = release == default && currentVersion == _lastCheckNonNullVersion;

                    if (release != default)
                        _lastCheckNonNullVersion = release.Version;
                    
                    if (isUpdated)
                    {
                        SetStatus("You are running newest version.", false, false);
                    }
                    else
                    {
                        if (AutoUpdate)
                        {
                            await updateTask.ContinueWith(x =>
                            {
                                updateManager.Dispose();
                                UpdateManager.RestartApp();
                            });
                        }
                        else
                        {
                            SetStatus("Restart app to install update", false, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus("Error while updating", false, false);
                _logger.Warn(ex, "Error while updating");
            }
        }
        

        private void SetStatus(string text = default, bool? showProgress = default, bool? showRestart = default, int? progress = default)
        {
            Execute.OnUIThread(() =>
            {
                if (text != default)
                    UpdateText = text;
                if (showProgress != default)
                    ProgressBarVisibility = showProgress.Value;
                if (showRestart != default)
                    ShowRestart = showRestart.Value;
                if (progress != default)
                    ProgressValue = progress.Value;
            });
        }
        
        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
            SettingsManager.HandleSPropertyChange<UpdateViewModel>(propertyName, LockConfig);
        }
        
        #endregion
    }
}