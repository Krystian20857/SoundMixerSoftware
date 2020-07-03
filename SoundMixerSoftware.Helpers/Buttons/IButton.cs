using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace SoundMixerSoftware.Helpers.Buttons
{
    public interface IButton
    {
        /// <summary>
        /// Defines name of button
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Set const key of button.
        /// </summary>
        string Key { get; }
        /// <summary>
        /// Index of assigned button.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// UUID of function.
        /// </summary>
        Guid UUID { get; set; }
        /// <summary>
        /// Defines image.
        /// </summary>
        ImageSource Image { get; set; }
        /// <summary>
        /// Saves button to container.
        /// </summary>
        /// <param name="iButton"></param>
        /// <returns></returns>
        Dictionary<object, object> Save();
        /// <summary>
        /// Occurs when button has been pushed down.
        /// </summary>
        /// <param name="index"></param>
        void ButtonKeyDown(int index);
        /// <summary>
        /// Occurs when button has been pulled up?.
        /// </summary>
        /// <param name="index"></param>
        void ButtonKeyUp(int index);
    }
}