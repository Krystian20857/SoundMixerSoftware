using System;
using System.Timers;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class SessionSlider : IVirtualSlider
    {
        #region Private Fields

        private readonly SimpleAudioVolume _simpleVolume;
        private readonly Timer _update;
        
        private float _lastVolume;
        private bool _lastMute;
        
        #endregion

        #region Public Properties

        public float Volume { get; set; }
        public bool IsMute { get; set; }

        public bool IsMasterVolume => false;
        public SliderType SliderType { get; }
        
        public string SessionID { get; }

        #endregion

        #region Constructor

        public SessionSlider(AudioSessionControl sessionControl)
        {
            SessionID = sessionControl.GetSessionIdentifier;
            _simpleVolume = sessionControl.SimpleAudioVolume;
            SliderType = SliderType.SESSION;

            _lastVolume = _simpleVolume.Volume;
            Volume = _lastVolume;

            _lastMute = _simpleVolume.Mute;
            IsMute = _lastMute;
            
            _update = new Timer();
            _update.Elapsed += UpdateOnElapsed;
            _update.Interval = 10;
            _update.AutoReset = true;
            _update.Start();
        }

        private void UpdateOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (Math.Abs(Volume - _lastVolume) >= SliderUtils.CHANGE_DIFF)
            {
                _lastVolume = Volume;
                _simpleVolume.Volume = _lastVolume;
            }

            if (IsMute != _lastMute)
            {
                _lastMute = IsMute;
                _simpleVolume.Mute = _lastMute;
            }
        }

        #endregion

        #region Private Events

        #endregion

        #region Dispose

        public void Dispose()
        {
            _update.Dispose();
        }

        #endregion

    }
}