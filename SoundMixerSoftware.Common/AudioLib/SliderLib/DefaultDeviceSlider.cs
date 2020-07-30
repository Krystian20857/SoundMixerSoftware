using System;
using System.Diagnostics;
using System.Timers;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Threading.Com;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DefaultDeviceSlider : IVirtualSlider
    {
        #region Private Fields

        private MMDevice _device;
        private readonly DeviceEnumerator _deviceEnumerator = new DeviceEnumerator();

        #endregion

        #region Public Properties

        public float Volume
        {
            get => ComThread.Invoke(() => _device.AudioEndpointVolume.MasterVolumeLevelScalar);
            set => SetVolumeInternal(value);
        }

        public bool IsMute
        {
            get => ComThread.Invoke(() => _device.AudioEndpointVolume.Mute);
            set => SetMuteInternal(value);
        }

        public bool IsMasterVolume => true;
        public SliderType SliderType { get; }

        public DataFlow DataFlow { get; set; }

        public string DeviceID => ComThread.Invoke(() => _device.ID);

        #endregion

        #region Constructor

        public DefaultDeviceSlider(SliderType sliderType, DataFlow dataFlow)
        {
            if (sliderType != SliderType.DEFAULT_MULTIMEDIA && SliderType != SliderType.DEFAULT_COMMUNICATION)
                throw new ArgumentException($"Cannot create default slider with: {sliderType} slider type.");
            
            DataFlow = dataFlow;
            SliderType = sliderType;

            _device = ComThread.Invoke(() => _deviceEnumerator.GetDefaultEndpoint(DataFlow, SliderType == SliderType.DEFAULT_MULTIMEDIA ? Role.Multimedia : Role.Communications));
            _deviceEnumerator.DefaultDeviceChange += (sender, args) => ComThread.Invoke(() =>
            {
                if (args.DataFlow != DataFlow)
                    return;
                var deviceId = sender as string;
                _device = _deviceEnumerator.GetDeviceById(deviceId);
            });
        }

        #endregion

        #region Private Methods

        internal void SetVolumeInternal(float volume)
        {
            try
            {
                ComThread.BeginInvoke(() => _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume);
            }
            finally { }
        }

        internal void SetMuteInternal(bool mute)
        {
            try
            {
                ComThread.BeginInvoke(() => _device.AudioEndpointVolume.Mute = mute);
            }
            finally { }
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