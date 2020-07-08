using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SoundMixerSoftware.Win32.Interop.Method
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool QueryFullProcessImageName([In]IntPtr hProcess, [In]int dwFlags, [Out]StringBuilder lpExeName, ref int lpdwSize);
    }
}