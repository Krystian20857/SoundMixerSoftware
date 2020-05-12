using System;

namespace SoundMixerSoftware.Win32.USBLib
{
    public class DeviceStateArgs : EventArgs
    {
        /// <summary>
        /// Device type.
        /// </summary>
        public uint DeviceType { get; set; }

        /// <summary>
        /// Connected/Disconnected DeviceProperties.
        /// </summary>
        public DeviceProperties DeviceProperties { get; set; }

        /// <summary>
        /// Create event args instance.
        /// </summary>
        /// <param name="deviceType"></param>
        public DeviceStateArgs(uint deviceType, DeviceProperties deviceProperties)
        {
            DeviceType = deviceType;
            DeviceProperties = deviceProperties;
        }
    }
}