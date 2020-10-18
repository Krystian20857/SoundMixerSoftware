using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Win32.Interop.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LARGE_INTEGER
    {
        public uint LowPart;
        public uint HighPart;
    }
}