using System;
using System.ComponentModel;
using System.Windows.Controls;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ManagerViewModel : ITabModel
    {
        #region Private Fields
        
        private BindableCollection<ProfileModel> _profiles = new BindableCollection<ProfileModel>();
        
        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion
        
        #region Public Properties

        public BindableCollection<ProfileModel> Profiles
        {
            get => _profiles;
            set => _profiles = value;
        }

        #endregion
        
        #region Constructor
        
        public ManagerViewModel()
        {
            Name = "Profiles";
            Icon = PackIconKind.AccountBoxMultipleOutline;
            
            Profiles.Add(new ProfileModel()
            {
                AttachedDevices = "Com5",
                ProfileName = "Profile1"
            });
        }
        
        #endregion
    }
}