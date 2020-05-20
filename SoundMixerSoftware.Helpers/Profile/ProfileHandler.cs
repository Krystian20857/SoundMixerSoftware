using System;
using SoundMixerSoftware.Common.Config;
using SoundMixerSoftware.Common.Config.Yaml;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.LocalSystem;

namespace SoundMixerSoftware.Helpers.Profile
{
    public static class ProfileHandler
    {
        #region Private Static Fields

        #endregion
        
        #region Public Static Properties
        
        public static readonly IProfileManager<ProfileStruct> ProfileManager = new YamlProfileManager<ProfileStruct>(LocalContainer.Profiles);
        
        #endregion
        
        #region Static Events

        public static event EventHandler<ProfileChangedEventArgs> ProfileChanged;
        
        private static void OnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            ConfigHandler.ConfigStruct.SelectedProfile = e.Uuid;
            ConfigHandler.SaveConfig();
        }
        
        #endregion
        
        #region Constructor

        static ProfileHandler()
        {
            ProfileManager.LoadAll();
            ProfileChanged += OnProfileChanged;
        }

        #endregion

        #region Public Static Methods

        public static void OnProfileChanged(Guid uuid)
        {
            ProfileChanged?.Invoke(null, new ProfileChangedEventArgs(uuid));
        }

        #endregion
    }

    public class ProfileChangedEventArgs : EventArgs
    {
        #region Public Proeprties

        /// <summary>
        /// UUID of profile.
        /// </summary>
        public Guid Uuid { get; set; }

        #endregion
        
        #region Constructor

        public ProfileChangedEventArgs(Guid uuid)
        {
            Uuid = uuid;
        }

        #endregion
    }
}