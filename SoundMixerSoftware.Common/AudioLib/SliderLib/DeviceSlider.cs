using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DeviceSlider : IVirtualSlider
    {

        #region Private Fields

        private readonly MMDevice _device;

        #endregion

        #region Public Properties

        public float Volume
        {
            get
            {
                return _device.AudioEndpointVolume.MasterVolumeLevelScalar;
            }
            set
            {
                if (value >= 0 && value <= 1)
                    _device.AudioEndpointVolume.MasterVolumeLevelScalar = value;
            }
        }
        public bool IsMute { get; set; }
        public bool IsMasterVolume => true;
        public SliderType SliderType { get; }
        public bool IsDefaultInput { get; }
        public bool IsDefaultOutput { get; }
        public string DeviceID => _device.ID;

        #endregion

        #region Constructor

        public DeviceSlider(MMDevice device)
        {
            _device = device;
            SliderType = device.DataFlow == DataFlow.Capture ? SliderType.MASTER_CAPTURE : SliderType.MASTER_RENDER;
        }
        
        public DeviceSlider(MMDevice device, bool isDefaultInput, bool isDefaultOutput): this(device)
        {
            IsDefaultInput = isDefaultInput;
            IsDefaultOutput = isDefaultOutput;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Dispose

        public void Dispose()
        {
        }

        #endregion

    }
}