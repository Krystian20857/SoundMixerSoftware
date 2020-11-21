using System;
using SoundMixerSoftware.Interop.USBLib;

namespace SoundMixerSoftware.Framework.Device
{
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