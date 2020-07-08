using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Win32.Interop.Method
{
    public static class Advapi
    {
        [DllImport("advapi32.dll", SetLastError=true)]
        public static extern int RegCloseKey(IntPtr hKey);
    }
}