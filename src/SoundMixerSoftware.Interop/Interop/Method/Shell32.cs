using System;
using System.Runtime.InteropServices;
using SoundMixerSoftware.Interop.Interface;

namespace SoundMixerSoftware.Interop.Method
{
    public static class Shell32
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        [DllImport("shell32.dll", EntryPoint = "ExtractIcon")]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        // ReSharper disable once InconsistentNaming
        public static extern IShellItem2 SHCreateItemInKnownFolder([MarshalAs(UnmanagedType.LPStruct)] Guid kfid, uint dwKFFlags, [MarshalAs(UnmanagedType.LPWStr)] string pszItem, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
    }
}