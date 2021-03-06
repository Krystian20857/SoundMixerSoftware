﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Buttons.Functions;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Utils;

namespace SoundMixerSoftware.ViewModels
{
    public class ProfileButtonViewModel : IButtonAddModel, INotifyPropertyChanged
    {
        #region Private Fields

        private EnumDisplayModel<ProfileFuncTask> _selectedFunction;
        
        #endregion
        
        #region Implemented Properties

        public string Name { get; set; } = "Profile Control";
        public PackIconKind Icon { get; set; } = PackIconKind.Account;
        public Guid UUID { get; set; } = new Guid("16724419-47BC-4C09-BB80-623F61DB5326");

        #endregion
        
        #region Public Properties

        public BindableCollection<EnumDisplayModel<ProfileFuncTask>> Functions { get; set; } = new BindableCollection<EnumDisplayModel<ProfileFuncTask>>();

        public EnumDisplayModel<ProfileFuncTask> SelectedFunction
        {
            get => _selectedFunction;
            set
            {
                _selectedFunction = value;
                ProfilesVisibility = value.EnumValue == ProfileFuncTask.SET_PROFILE;
                OnPropertyChanged(nameof(ProfilesVisibility));
            }
        }

        public BindableCollection<ProfileModel> Profiles => ManagerViewModel.Instance.Profiles;
        public ProfileModel SelectedProfile { get; set; }

        public bool ProfilesVisibility { get; set; }

        #endregion
        
        #region Constructor

        public ProfileButtonViewModel()
        {
            EnumDisplayHelper.AddItems(Functions);

            SelectedFunction = Functions[0];
        }
        
        #endregion
        
        #region Implemented Methods
        
        public bool AddClicked(int index)
        {
            var function = (IButtonFunction)null;
            switch (SelectedFunction.EnumValue)
            {
                case ProfileFuncTask.CYCLE:
                    function = new ProfileFunction(index, Guid.NewGuid());
                    break;
                case ProfileFuncTask.SET_PROFILE:
                    if (SelectedProfile == null)
                        return false;
                    function = new ProfileFunction(index, SelectedProfile.Guid, Guid.NewGuid());
                    break;
                default:
                    return false;
            }
            
            ButtonUtil.AddButton(index, function);

            return true;
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