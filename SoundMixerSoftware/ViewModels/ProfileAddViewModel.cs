using Caliburn.Micro;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ProfileAddViewModel : Screen
    {
        #region Private Fields

        private bool _editMode = false;

        #endregion
        
        #region Public Properties
        
        public ProfileModel CreatedProfile { get; set; }
        public string Title { get; set; }

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
                ProfileHandler.ProfileManager.Profiles[model.Guid] = profile;
                ProfileHandler.ProfileManager.Save(model.Guid);
                CreatedProfile = model;
            }
            else
            {
                model.Guid = ProfileHandler.ProfileManager.Create(profile);
                CreatedProfile = model;
            }

            TryClose();
        }

        /// <summary>
        /// Occurs when Cancel button has clicked.
        /// </summary>
        public void CancelClick()
        {
            TryClose();
        }
        
        #endregion
    }
}