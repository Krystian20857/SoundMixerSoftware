﻿using System;
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
        /// <summary>
        /// True if device has detected on application startup.
        /// </summary>
        public bool DetectedOnStartup { get; set; }

        public DeviceConnectedEventArgs(DeviceProperties device, DeviceIdResponse deviceResponse, bool detectedOnStartup)
        {
            Device = device;
            DeviceResponse = deviceResponse;
            DetectedOnStartup = detectedOnStartup;
        }
    }
}