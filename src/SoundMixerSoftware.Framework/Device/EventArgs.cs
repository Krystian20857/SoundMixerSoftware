using System;
using SoundMixerSoftware.Win32.USBLib;

namespace SoundMixerSoftware.Framework.Device
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

    public class SliderValueChanged : EventArgs
    {
        public DeviceId DeviceId { get; set; }
        public int Index { get; set; }
        public float Value { get; set; }

        public SliderValueChanged(DeviceId deviceId, int index, float value)
        {
            DeviceId = deviceId;
            Index = index;
            Value = value;
        }
    }

    public class ButtonStateChanged : EventArgs
    {
        public DeviceId DeviceId { get; set; }
        public int Index { get; set; }
        public ButtonState State { get; set; }

        public ButtonStateChanged(DeviceId deviceId, int index, ButtonState state)
        {
            DeviceId = deviceId;
            Index = index;
            State = state;
        }
    }
}