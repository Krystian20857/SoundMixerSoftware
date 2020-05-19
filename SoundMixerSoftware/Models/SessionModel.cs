using System.Windows.Media;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Model use to creating SessionSlider.
    /// </summary>
    public class SessionModel
    {
        /// <summary>
        /// Name of audio session.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Image of audio session.
        /// </summary>
        public ImageSource Image { get; set; }
        /// <summary>
        /// Process Id of session.
        /// </summary>
        public int ProcessID { get; set; }
    }
}