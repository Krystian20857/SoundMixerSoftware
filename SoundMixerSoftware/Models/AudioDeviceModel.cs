using System.Windows.Media;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Contains data needed to create master volume slider.
    /// </summary>
    public class AudioDeviceModel
    {
        /// <summary>
        /// Device name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Device Icon
        /// </summary>
        public ImageSource Image { get; set; }
        /// <summary>
        /// Device string ID.
        /// </summary>
        public string Id { get; set; }
    }
}