using System;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Threading.Com;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DeviceSlider : IVirtualSlider
    {
        #region Private Fields

        private MMDevice _device;

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
        public SliderType SliderType { get; } = SliderType.SESSION;
        public string DeviceID => ComThread.Invoke(() => _device.ID);

        #endregion

        #region Constructor

        public DeviceSlider(string deviceId)
        {
            _device = ComThread.Invoke(() => new MMDeviceEnumerator().GetDevice(deviceId));
        }

        #endregion

        #region Private Methods

        internal void SetVolumeInternal(float volume)
        {
            try
            {
                ComThread.BeginInvoke(() => _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume);
            }finally { }
        }
        
        internal void SetMuteInternal(bool mute)
        {
            try
            {
                ComThread.BeginInvoke(() => _device.AudioEndpointVolume.Mute = mute);
            }finally { }
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