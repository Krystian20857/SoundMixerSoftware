using System;
using NAudio.CoreAudioApi;
using NAudio.MediaFoundation;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class SessionSlider : IVirtualSlider
    {
        #region Private Fields

        #endregion
        
        #region Implemented Properties

        public float Volume
        {
            get => SessionControl.SimpleAudioVolume.Volume;
            set => SessionControl.SimpleAudioVolume.Volume = value;
        }
        public bool IsMute
        {
            get => SessionControl.SimpleAudioVolume.Mute;
            set => SessionControl.SimpleAudioVolume.Mute = value;
        }

        public bool IsMasterVolume => false;
        public SliderType SliderType => SliderType.SESSION;
        public AudioSessionControl SessionControl { get; }

        #endregion
        
        #region Constructor

        public SessionSlider(AudioSessionControl session)
        {
            SessionControl = session;
        }
        
        #endregion
        
        #region Dispose

        public void Dispose()
        {
            //_session?.Dispose();
        }
        
        #endregion
    }
}