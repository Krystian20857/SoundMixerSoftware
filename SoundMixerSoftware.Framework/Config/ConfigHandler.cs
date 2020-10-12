using SoundMixerSoftware.Common.Config;
using SoundMixerSoftware.Common.Config.Yaml;
using SoundMixerSoftware.Helpers.LocalSystem;

namespace SoundMixerSoftware.Helpers.Config
{
    /// <summary>
    /// Handles Global Config
    /// </summary>
    public static class ConfigHandler
    {
        #region Public Properties

        /// <summary>
        /// Global config Struct.
        /// </summary>
        public static ConfigStruct ConfigStruct
        {
            get => Config.Config;
            set => Config.Config = value;
        }
        
        /// <summary>
        /// Global Config Struct Handler.
        /// </summary>
        public static IConfig<ConfigStruct> Config { get; set; }

        #endregion
        
        #region Static Constructor

        static ConfigHandler()
        {
            Initialize();
        }
        
        #endregion
        
        #region Public Static Methods

        /// <summary>
        /// Initialize config.
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            if (Config != null) return false;
            Config = new YamlConfig<ConfigStruct>(LocalContainer.Config);
            return true;
        }

        /// <summary>
        /// SaveConfig Handler passthroughs
        /// </summary>
        public static void SaveConfig() => Config.SaveConfig();
        /// <summary>
        /// LoadConfig Handler passthroughs
        /// </summary>
        public static void LoadConfig() => Config.LoadConfig();

        /// <summary>
        /// Check whatever Config Handler is initialized.
        /// </summary>
        /// <returns></returns>
        public static bool IsInitialize() => Config != null;

        #endregion
    }
}