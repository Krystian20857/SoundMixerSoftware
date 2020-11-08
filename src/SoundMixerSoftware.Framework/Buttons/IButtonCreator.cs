using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Framework.Buttons
{
    public interface IButtonCreator
    {
        /// <summary>
        /// Creates iButton instance from container.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        IButtonFunction CreateButton(int index, Dictionary<object, object> container, Guid uuid);
    }
}