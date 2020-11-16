using System;
using SoundMixerSoftware.Common.Config;
using SoundMixerSoftware.Common.Config.Yaml;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.LocalSystem;

namespace SoundMixerSoftware.Framework.Profile
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

        /// <summary>
        /// Occurs when selected profile has changed.
        /// </summary>
        public static event EventHandler<ProfileChangedEventArgs> ProfileChanged;

        public static event EventHandler<ProfileChangedEventArgs> ProfileCreated;
        public static event EventHandler<ProfileChangedEventArgs> ProfileRemoved;

        private static void OnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            var saveConfig = !(SelectedGuid == Guid.Empty && e.Uuid != Guid.Empty);
            SelectedGuid = e.Uuid;
            SelectedProfile = ProfileManager.Profiles[e.Uuid];
            ConfigHandler.ConfigStruct.Application.SelectedProfile = e.Uuid;
            if(saveConfig)
                ConfigHandler.SaveConfig();
        }

        #endregion
        
        #region Constructor

        static ProfileHandler()
        {
            ProfileChanged += OnProfileChanged;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Invoke profile change event.
        /// </summary>
        /// <param name="uuid"></param>
        public static void OnProfileChanged(Guid uuid)
        {
            SelectedGuid = uuid;
            ProfileChanged?.Invoke(null, new ProfileChangedEventArgs(uuid));
        }

        public static void OnProfileCreated(Guid uuid)
        {
            ProfileCreated?.Invoke(null, new ProfileChangedEventArgs(uuid));
        }
        
        public static void OnProfileRemoved(Guid uuid)
        {
            ProfileRemoved?.Invoke(null, new ProfileChangedEventArgs(uuid));
        }

        /// <summary>
        /// Save currently selected profile.
        /// </summary>
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