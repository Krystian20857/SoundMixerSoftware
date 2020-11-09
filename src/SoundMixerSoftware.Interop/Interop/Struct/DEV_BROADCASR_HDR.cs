using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace SoundMixerSoftware.Interop.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_HDR
    {
        public uint dbch_Size;
        public uint dbch_DeviceType;
        public uint dbch_Reserved;
    }
}