namespace SoundMixerSoftware.Common.Config
{
    /// <summary>
    /// Represents base config structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConfig<T> where T : IConfigStruct<T>
    {
        /// <summary>
        /// Properties that holds loaded config.
        /// </summary>
        T Config { get; set; }
        /// <summary>
        /// Loads config to <see cref="Config"/> property.
        /// </summary>
        void LoadConfig();
        /// <summary>
        /// Saves config to file from <see cref="Config"/> property.
        /// </summary>
        void SaveConfig();
        /// <summary>
        /// Save sample config.
        /// </summary>
        void SaveSampleConfig();
    }
}