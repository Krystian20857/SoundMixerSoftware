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

        public static IProfileManager<ProfileStruct> ProfileManager { get; } = new YamlProfileManager<ProfileStruct>(LocalContainer.Profiles);

        public static ProfileStruct SelectedProfile { get; set; } = new ProfileStruct();
        public static Guid SelectedGuid { get; set; }

        #endregion
        
        #region Static Events

        public static event EventHandler<ProfileChangedEventArgs> ProfileChanged;
        
        private static void OnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            SelectedGuid = e.Uuid;
            SelectedProfile = ProfileManager.Profiles[e.Uuid];
            ConfigHandler.ConfigStruct.Application.SelectedProfile = e.Uuid;
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
            SelectedGuid = uuid;
            ProfileChanged?.Invoke(null, new ProfileChangedEventArgs(uuid));
        }

        public static void SaveSelectedProfile()
        {
            ProfileManager.Save(SelectedGuid);
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