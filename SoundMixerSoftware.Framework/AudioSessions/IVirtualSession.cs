using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace SoundMixerSoftware.Framework.AudioSessions
{
    public interface IVirtualSession : IDisposable
    {
        /// <summary>
        /// Key for identifying slider.
        /// </summary>
        string Key { get; }
        /// <summary>
        /// Display name of slider.
        /// </summary>
        string DisplayName { get; set; }
        /// <summary>
        /// Id of slider.
        /// </summary>
        string ID { get; }
        /// <summary>
        /// Index of current session.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// Unique identifier od virtual session.
        /// </summary>
        Guid UUID { get; }
        /// <summary>
        /// Image of slider.
        /// </summary>
        ImageSource Image { get; set; }
        /// <summary>
        /// Current state of slider.
        /// </summary>
        SessionState State { get; set; }

        /// <summary>
        /// Sets and gets volume level of session.
        /// </summary>
        float Volume { get; set; }
        /// <summary>
        /// Sets and gets mute of session.
        /// </summary>
        bool IsMute { get; set; }

        event EventHandler<VolumeChangedArgs> VolumeChange;
        event EventHandler<MuteChangedArgs> MuteChanged;

        /// <summary>
        /// Save virtual session to container.
        /// </summary>
        /// <returns></returns>
        Dictionary<object, object> Save();
    }
}