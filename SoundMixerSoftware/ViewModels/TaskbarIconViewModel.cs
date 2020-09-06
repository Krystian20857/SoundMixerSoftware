using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class TaskbarIconViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private string _profileName;

        #endregion
        
        #region Public Properties
        
        //public BindableCollection<ProfileModel> Profiles { get; set; } = new BindableCollection<ProfileModel>();

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

        #endregion
        
        #region Constructor

        public TaskbarIconViewModel()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ProfileHandler.ProfileChanged += (sender, args) =>
            {
                ProfileName = ProfileHandler.SelectedProfile.Name;
            };
            
        }

        #endregion
        
        #region Private Events

        public void ShowWindow()
        {
            Bootstrapper.Instance.BringToFront();
        }

        public void ExitApp()
        {
            Application.Current.Shutdown();
        }

        public void RestartApp()
        {
            StarterHelper.RestartApp();
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