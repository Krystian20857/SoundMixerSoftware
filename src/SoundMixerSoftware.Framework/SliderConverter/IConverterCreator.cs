using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Framework.SliderConverter
{
    public interface IConverterCreator
    {
        /// <summary>
        /// Create converter from container.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="container"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        IConverter CreateConverter(int index, Dictionary<object, object> container, Guid uuid);
    }
}