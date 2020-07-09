using System.ComponentModel;
using System.Windows.Forms;
using Caliburn.Micro;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;
using Screen = Caliburn.Micro.Screen;

namespace SoundMixerSoftware.ViewModels
{
    public class ProfileAddViewModel : Screen
    {
        #region Private Fields

        private readonly bool _editMode;
        private DeviceModel _selectedDevice;

        #endregion
        
        #region Public Properties
        
        public ProfileModel CreatedProfile { get; set; }
        public string Title { get; set; }
        public BindableCollection<DeviceModel> Devices { get; set; } = new BindableCollection<DeviceModel>();

        public DeviceModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                if (Devices.IndexOf(value) < 0)
                    return;
                CreatedProfile.ProfileName = value.Name;
                CreatedProfile.ButtonCount = value.Buttons;
                CreatedProfile.SliderCount = value.Sliders;
                NotifyOfPropertyChange(nameof(CreatedProfile));
            }
        }

        #endregion
        
        #region Constructor

        public ProfileAddViewModel(ProfileModel model = null)
        {
            _editMode = model != null;
            if (_editMode)
            {
                Title = "Edit Profile";
                CreatedProfile = (ProfileModel)model.Clone();
            }
            else
            {
                Title = "Add Profile";
                CreatedProfile = new ProfileModel();
            }
            
            foreach (var device in DeviceHandlerGlobal.ConnectedDevice)
                Devices.Add(DeviceModel.CreateModel(device.Value));
        }
        
        #endregion
        
        #region Private Events

        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        public void AddClick()
        {
            var profile = ProfileModel.CreateProfile(CreatedProfile);
            var model = ProfileModel.CreateModel(profile);
            if (_editMode)
            {
                model.Guid = CreatedProfile.Guid;
                var profileToEdit = ProfileHandler.ProfileManager.Profiles[model.Guid];
                profileToEdit.ButtonCount = profile.ButtonCount;
                profileToEdit.SliderCount = profile.SliderCount;
                ProfileHandler.ProfileManager.Save(model.Guid);
                CreatedProfile = model;
            }
            else
            {
                model.Guid = ProfileHandler.ProfileManager.Create(profile);
                model.Selected = true;
                ProfileHandler.OnProfileChanged(model.Guid);
                CreatedProfile = model;
            }

            TryCloseAsync();
        }

        /// <summary>
        /// Occurs when Cancel button has clicked.
        /// </summary>
        public void CancelClick()
        {
            TryCloseAsync();
        }
        
        #endregion
    }
}