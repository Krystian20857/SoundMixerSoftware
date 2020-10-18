using System;

namespace SoundMixerSoftware.Common.Config
{
    public interface IConfigStruct<T> : ICloneable
    {
        /// <summary>
        /// Use for getting SampleConfig use in object merging and first time config creation.
        /// </summary>
        T GetSampleConfig();
    }
}