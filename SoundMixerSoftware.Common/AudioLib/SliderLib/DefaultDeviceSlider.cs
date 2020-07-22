using System.Windows.Threading;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DefaultDeviceSlider : IVirtualSlider
    {
        #region Private Fields
        
        private MMDevice _device;
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private readonly DeviceEnumerator _deviceEnumerator = new DeviceEnumerator();
        private readonly DataFlow _dataFlow;

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
        public bool IsDefaultOutput { get; }
        public string DeviceID => _dispatcher.Invoke(() => _device.ID);

        #endregion

        #region Constructor

        public DefaultDeviceSlider(bool isDefaultOutput)
        {
            IsDefaultOutput = isDefaultOutput;
            _dataFlow = isDefaultOutput ? DataFlow.Render : DataFlow.Capture;
            SliderType = isDefaultOutput ? SliderType.MASTER_RENDER : SliderType.MASTER_CAPTURE;

            _dispatcher.Invoke(() =>
            {
                _device = _deviceEnumerator.GetDefaultEndpoint(_dataFlow, Role.Multimedia);
            });
            _deviceEnumerator.DefaultDeviceChange += (sender, args) =>
            {
                if (args.DataFlow != _dataFlow)
                    return;
                _dispatcher.Invoke(() =>
                {
                    _device = _deviceEnumerator.GetDeviceById(sender as string);
                });
            };
        }

        #endregion

        #region Private Methods

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