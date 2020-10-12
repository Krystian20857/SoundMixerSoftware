using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Helpers.Device
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DeviceIdRequest
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.I1)] public Command command;
        [FieldOffset(1)] public byte flag;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceIdResponse
    {
        [MarshalAs(UnmanagedType.U1)] public Command command;
        [MarshalAs(UnmanagedType.U1)] public byte flag;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public byte[] uuid;
        [MarshalAs(UnmanagedType.U1)] public byte slider_count;
        [MarshalAs(UnmanagedType.U1)] public byte button_count;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SliderStruct
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.I1)] public Command command;
        [FieldOffset(1)] public byte slider;
        [FieldOffset(2)] public ushort value;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct ButtonStruct
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.I1)] public Command command;
        [FieldOffset(1)] public byte button;
        [FieldOffset(2)] public byte state;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct LedStruct
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.I1)] public Command command;
        [FieldOffset(1)] public byte led;
        [FieldOffset(2)] public byte state;
    }

    [Flags]
    public enum Command : byte
    {
        /// <summary>
        /// Out command.
        /// </summary>
        SLIDER_COMMAND = 0x01,
        /// <summary>
        /// Out command.
        /// </summary>
        BUTTON_COMMAND = 0x02,
        /// <summary>
        /// Out command.
        /// </summary>
        DEVICE_RESPONSE_COMMAND = 0x03,
        
        /// <summary>
        /// In command.
        /// </summary>
        DEVICE_REQUEST_COMMAND = 0x02,
        /// <summary>
        /// In command.
        /// </summary>
        LED_COMMAND = 0x01
    }
}