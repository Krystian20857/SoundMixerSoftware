using System;
using System.Runtime.InteropServices;

namespace SoundMixerAppv2.Win32.Win32
{
    public static class NativeMethods
    {
        [DllImport("shell32.dll", CharSet=CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex,out IntPtr phiconLarge,out IntPtr phiconSmall, uint nIcons);
        
        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput (uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] NativeStructs.INPUT[] pInputs, int cbSize);
        
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}