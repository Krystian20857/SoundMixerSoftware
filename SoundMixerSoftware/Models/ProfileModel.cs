using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Helpers.Profile;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Model of profile 
    /// </summary>
    public class ProfileModel : ICloneable, INotifyPropertyChanged
    {
        #region Private Fields

        private bool _selected = false;
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Profile Name
        /// </summary>
        public string ProfileName { get; set; }
        /// <summary>
        /// Sliders attached to profile.
        /// </summary>
        public int SliderCount { get; set; }
        /// <summary>
        /// Buttons attached to profile.
        /// </summary>
        public int ButtonCount { get; set; }
        /// <summary>
        /// Profile Guid
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Is selected.
        /// </summary>
        /// <returns></returns>
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Check if profile is initialized.
        /// </summary>
        /// <returns></returns>
        public bool IsInitialized() => Guid != Guid.Empty;
        
        /// <summary>
        /// Clone object...
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        
        #endregion
        
        #region Public Static Methods

        /// <summary>
        /// Create profile model from profile struct
        /// </summary>
        /// <param name="profileStruct"></param>
        /// <returns></returns>
        public static ProfileModel CreateModel(ProfileStruct profileStruct)
        {
            return new ProfileModel
            {
                ProfileName = profileStruct.Name,
                ButtonCount = profileStruct.ButtonCount,
                SliderCount = profileStruct.SliderCount,
            };
        }

        /// <summary>
        /// Create profile form profile model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ProfileStruct CreateProfile(ProfileModel model)
        {
            return new ProfileStruct
            {
                Name = model.ProfileName,
                SliderCount = model.SliderCount,
                ButtonCount = model.ButtonCount,
            };
        }
        
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}