using System.Windows.Media;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Helpers.AudioSessions;

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
        public string ID { get; set; }
        /// <summary>
        /// Represents mode of session.
        /// </summary>
        public SessionMode SessionMode { get; set; }
        /// <summary>
        /// Session DataFlow
        /// </summary>
        public DataFlow DataFlow { get; set; }
        /// <summary>
        /// Is Active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}