using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Buttons.Functions;
using SoundMixerSoftware.Framework.Profile;
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
            
            var buttonStruct = ButtonHandler.AddFunction(index, function);
            ProfileHandler.SelectedProfile.Buttons[index].Functions.Add(buttonStruct);
            ProfileHandler.SaveSelectedProfile();

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