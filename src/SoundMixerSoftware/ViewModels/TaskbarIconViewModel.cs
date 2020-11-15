using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Models;
using Squirrel;

namespace SoundMixerSoftware.ViewModels
{
    public class TaskbarIconViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private string _profileName;

        #endregion
        
        #region Properties

        public BindableCollection<ProfileModel> Profiles => ManagerViewModel.Instance.Profiles;

        public string ProfileName
        {
            get => _profileName;
            set
            {
                _profileName = value;
                OnPropertyChanged(nameof(ProfileName));
            }
        }

        private TaskbarIcon Resource => Bootstrapper.Instance.TaskbarIcon;

        #endregion
        
        #region Constructor

        public TaskbarIconViewModel()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ProfileHandler.ProfileChanged += (sender, args) =>
            {
                ProfileName = ProfileHandler.SelectedProfile.Name;
            };
            
            Bootstrapper.Instance.ViewInitialized += InstanceOnViewInitialized;
        }

        #endregion
        
        #region Private Events
        
        private void InstanceOnViewInitialized(object _, EventArgs e)
        {
            Resource.TrayLeftMouseUp += (sender, args) => ShowWindow();
            ThemeManager.ThemeChanged += (sender, args) => Resource.UpdateDefaultStyle();
        }

        public void ShowWindow()
        {
            Bootstrapper.Instance.SetForeground();
        }

        public void ExitApp()
        {
            Application.Current.Shutdown();
        }

        public void RestartApp()
        {
            UpdateManager.RestartApp();
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