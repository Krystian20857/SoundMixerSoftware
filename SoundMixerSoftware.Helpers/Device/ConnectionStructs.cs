using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Helpers.Device
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DeviceIdRequest
    {
        [FieldOffset(0)] public byte command;
        [FieldOffset(1)] public byte flag;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceIdResponse
    {
        [MarshalAs(UnmanagedType.U1)] public byte command;
        [MarshalAs(UnmanagedType.U1)] public byte flag;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public byte[] uuid;
        [MarshalAs(UnmanagedType.U1)] public byte slider_count;
        [MarshalAs(UnmanagedType.U1)] public byte button_count;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SliderStruct
    {
        [FieldOffset(0)] public byte command;
        [FieldOffset(1)] public byte slider;
        [FieldOffset(2)] public ushort value;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct ButtonStruct
    {
        [FieldOffset(0)] public byte command;
        [FieldOffset(1)] public byte button;
        [FieldOffset(2)] public byte state;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct LedStruct
    {
        [FieldOffset(0)] public byte command;
        [FieldOffset(1)] public byte led;
        [FieldOffset(2)] public byte state;
    }
}