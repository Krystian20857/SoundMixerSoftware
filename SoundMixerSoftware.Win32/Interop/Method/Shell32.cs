using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Win32.Interop.Method
{
    public static class Shell32
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);
        
        [DllImport("shell32.dll", EntryPoint = "ExtractIcon")]
        public extern static IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
    }
}