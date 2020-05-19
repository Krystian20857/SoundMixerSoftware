using System;
using SoundMixerSoftware.Win32.USBLib;

namespace SoundMixerSoftware.Helpers.Device
{
    public class DeviceConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Properties of device.
        /// </summary>
        public DeviceProperties Device { get; set; }
        /// <summary>
        /// Device id.
        /// </summary>
        public DeviceIdResponse DeviceResponse { get; set; }

        public DeviceConnectedEventArgs(DeviceProperties device, DeviceIdResponse deviceResponse)
        {
            Device = device;
            DeviceResponse = deviceResponse;
        }
    }
}