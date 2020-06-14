using System;
using System.Diagnostics;
using System.Timers;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DefaultDeviceSlider : IVirtualSlider
    {
        #region Private Fields
        
        private readonly Timer _update;
        private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        private readonly DataFlow _dataFlow;
        
        private float _lastVolume;
        private bool _lastMute;

        #endregion

        #region Public Properties

        public float Volume { get; set; }
        public bool IsMute { get; set; }
        public bool IsMasterVolume => true;
        public SliderType SliderType { get; }
        public bool IsDefaultOutput { get; }
        public string DeviceID { get; }

        #endregion

        #region Constructor

        public DefaultDeviceSlider(bool isDefaultOutput)
        {
            IsDefaultOutput = isDefaultOutput;
            _dataFlow = isDefaultOutput ? DataFlow.Render : DataFlow.Capture;
            SliderType = isDefaultOutput ? SliderType.MASTER_RENDER : SliderType.MASTER_CAPTURE;
            
            Volume = _lastVolume;
            
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
                var device = _deviceEnumerator.GetDefaultAudioEndpoint(_dataFlow, Role.Multimedia);
                
                _lastVolume = Volume;
                _lastMute = IsMute;
                
                device.AudioEndpointVolume.MasterVolumeLevelScalar = _lastVolume;
                device.AudioEndpointVolume.Mute = _lastMute;
            }

            if (IsMute != _lastMute)
            {
                var device = _deviceEnumerator.GetDefaultAudioEndpoint(_dataFlow, Role.Multimedia);
                
                _lastMute = IsMute;
                
                device.AudioEndpointVolume.Mute = _lastMute;
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