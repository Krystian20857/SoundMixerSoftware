using YamlDotNet.Serialization;

namespace SoundMixerSoftware.Common.Config
{
    public interface IConfigStruct<T>
    {
        /// <summary>
        /// Use for getting SampleConfig use in object merging and first time config creation.
        /// </summary>
        [YamlIgnore]
        T SampleConfig { get; }

        /// <summary>
        /// Suggested for creating Memberwise copy of object.
        /// </summary>
        /// <returns>Object copy.</returns>
        T Copy();
    }
}