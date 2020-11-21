using SoundMixerSoftware.Interop.USBLib;

namespace SoundMixerSoftware.Framework.Device
{
    public class DevicePair
    {
        /// <summary>
        /// Properties of device.
        /// </summary>
        public DeviceProperties Device { get; set; }
        /// <summary>
        /// Device id.
        /// </summary>
        public DeviceIdResponse DeviceResponse { get; set; }
        /// <summary>
        /// True if device has detected on application startup.
        /// </summary>
        public bool DetectedOnStartup { get; set; }

        public DevicePair(DeviceProperties device, DeviceIdResponse deviceResponse, bool detectedOnStartup)
        {
            Device = device;
            DeviceResponse = deviceResponse;
            DetectedOnStartup = detectedOnStartup;
        }
    }
}