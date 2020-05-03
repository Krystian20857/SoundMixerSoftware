namespace SoundMixerAppv2.Common.Config
{
    public interface IConfigStruct<out T>
    {
        /// <summary>
        /// Use for getting SampleConfig use in object merging and first time config creation.
        /// </summary>
        /// <returns>Sample Config.</returns>
        T GetSampleConfig();
        /// <summary>
        /// Suggested for creating Memberwise copy of object.
        /// </summary>
        /// <returns>Object copy.</returns>
        T Copy();
    }
}