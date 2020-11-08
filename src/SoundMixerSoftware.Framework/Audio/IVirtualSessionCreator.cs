using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Framework.Audio
{
    public interface IVirtualSessionCreator
    {
        /// <summary>
        /// Create session from raw data.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="container"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid);
    }
}