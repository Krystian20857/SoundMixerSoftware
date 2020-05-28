using System.Timers;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DeviceSlider : IVirtualSlider
    {
        #region Private Fields

        private readonly MMDevice _device;
        private readonly Timer _update;
        
        private float _lastVolume;
        private bool _lastMute;
        
        #endregion

        #region Public Properties

        public float Volume { get; set; }
        public bool IsMute { get; set; }
        public bool IsMasterVolume => true;
        public SliderType SliderType { get; }
        public string DeviceID { get; }

        #endregion

        #region Constructor

        public DeviceSlider(MMDevice device)
        {
            _device = device;
            SliderType = device.DataFlow == DataFlow.Capture ? SliderType.MASTER_CAPTURE : SliderType.MASTER_RENDER;

            _lastVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar;
            Volume = _lastVolume;

            _lastMute = _device.AudioEndpointVolume.Mute;
            IsMute = _lastMute;
            
            _update = new Timer();
            _update.Elapsed += UpdateOnElapsed;
            _update.Interval = 10;
            _update.AutoReset = true;
            _update.Start();
        }

        private void UpdateOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (Volume != _lastVolume)
            {
                _lastVolume = Volume;
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = _lastVolume;
            }

            if (IsMute != _lastMute)
            {
                _lastMute = IsMute;
                _device.AudioEndpointVolume.Mute = _lastMute;
            }
        }

        #endregion

        #region Private Events

        #endregion

        #region Dispose

        public void Dispose()
        {
            _update.Dispose();
            _device.Dispose();
        }

        #endregion

    }
}