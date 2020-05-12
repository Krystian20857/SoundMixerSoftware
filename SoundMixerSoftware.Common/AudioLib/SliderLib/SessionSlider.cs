using System;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class SessionSlider : IVirtualSlider
    {
        #region Private Fields

        private readonly AudioSessionControl _session;
        
        #endregion
        
        #region Implemented Properties

        public float Volume
        {
            get => _session.SimpleAudioVolume.Volume;
            set => _session.SimpleAudioVolume.Volume = value;
        }
        public bool IsMute
        {
            get => _session.SimpleAudioVolume.Mute;
            set => _session.SimpleAudioVolume.Mute = value;
        }

        public bool IsMasterVolume => false;
        public SliderType SliderType => SliderType.SESSION;
        
        #endregion
        
        #region Constructor

        public SessionSlider(AudioSessionControl session)
        {
            _session = session;
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