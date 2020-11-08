using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Win32.Interop.Method
{
    public static class Psapi
    {
        [DllImport("Psapi.dll", SetLastError = true)]
        public static extern bool EnumProcesses([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In] [Out] uint[] processIds, uint arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out uint bytesCopied);
    }
}