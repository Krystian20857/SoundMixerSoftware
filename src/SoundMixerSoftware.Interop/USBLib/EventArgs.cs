using System;

namespace SoundMixerSoftware.Interop.USBLib
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
        /// <param name="deviceProperties"></param>
        public DeviceStateArgs(uint deviceType, DeviceProperties deviceProperties)
        {
            DeviceType = deviceType;
            DeviceProperties = deviceProperties;
        }
    }
}