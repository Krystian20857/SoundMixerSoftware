using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace SoundMixerSoftware.Win32.Interop.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_DEVICEINTERFACE
    {
        public int dbcc_size;
        public int dbcc_devicetype;
        public int dbcc_reserved;
        [MarshalAs(UnmanagedType.Struct, SizeConst = 16)]
        public Guid dbcc_classguid;
        public char dbcc_name;
    }
}