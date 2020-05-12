using System;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public interface IVirtualSlider : IDisposable
    {
        /// <summary>
        /// Gets and Sets Volume of current virtual slider.
        /// </summary>
        float Volume { get; set; }
        /// <summary>
        /// Gets and Sets Mute of default slider.
        /// </summary>
        bool IsMute { get; set; }
        /// <summary>
        /// Gets if virtual slider handles master session.
        /// </summary>
        bool IsMasterVolume { get; }
        /// <summary>
        /// Gets virtual slider type.
        /// </summary>
        SliderType SliderType { get; }
    }
}