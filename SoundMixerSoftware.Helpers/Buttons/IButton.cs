using System.Collections.Generic;

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
        /// Store data from profile file and helps while saving.
        /// </summary>
        Dictionary<object, object> Container { get; }
        /// <summary>
        /// Saves button to container.
        /// </summary>
        /// <param name="iButton"></param>
        /// <returns></returns>
        Dictionary<object, object> Save();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void ButtonPressed(int index);
    }
}