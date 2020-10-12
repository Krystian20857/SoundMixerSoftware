using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Helpers.Buttons
{
    public interface IButtonCreator
    {
        /// <summary>
        /// Creates iButton instance from container.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        IButton CreateButton(int index, Dictionary<object, object> container, Guid uuid);
    }
}