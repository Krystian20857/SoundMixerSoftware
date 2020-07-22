using System.Timers;
using System.Windows.Threading;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DeviceSlider : IVirtualSlider
    {
        #region Private Fields

        private MMDevice _device;
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        #endregion

        #region Public Properties

        public float Volume
        {
            get => _dispatcher.Invoke(()=> _device.AudioEndpointVolume.MasterVolumeLevelScalar);
            set => SetVolumeInternal(value);
        }

        public bool IsMute
        {
            get => _dispatcher.Invoke(()=> _device.AudioEndpointVolume.Mute);
            set => SetMuteInternal(value);
        }
        public bool IsMasterVolume => true;
        public SliderType SliderType { get; }
        public string DeviceID => _dispatcher.Invoke(() => _device.ID);

        #endregion

        #region Constructor

        public DeviceSlider(MMDevice device)
        {
            _device = device;
            SliderType = device.DataFlow == DataFlow.Capture ? SliderType.MASTER_CAPTURE : SliderType.MASTER_RENDER;
        }

        #endregion

        #region Private Events
        
        internal void SetVolumeInternal(float volume)
        {
            _dispatcher.Invoke(() =>
            {
                try
                {
                    _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
                }
                finally { }
            });
        }

        internal void SetMuteInternal(bool mute)
        {
            _dispatcher.Invoke(() =>
            {
                try
                {
                    _device.AudioEndpointVolume.Mute = mute;
                }
                finally { }
            });
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _device?.Dispose();
        }

        #endregion

    }
}