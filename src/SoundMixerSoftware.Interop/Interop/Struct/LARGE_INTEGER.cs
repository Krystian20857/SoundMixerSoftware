using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace SoundMixerSoftware.Interop.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LARGE_INTEGER
    {
        public uint LowPart;
        public uint HighPart;
    }
}