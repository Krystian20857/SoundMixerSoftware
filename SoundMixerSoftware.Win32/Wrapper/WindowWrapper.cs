using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SoundMixerSoftware.Win32.Win32;

namespace SoundMixerSoftware.Win32.Wrapper
{
    /// <summary>
    /// Contains useful methods with windows.(bring to front, minimaze, close, etc)
    /// </summary>
    public class WindowWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns false when process not contains main window.</returns>
        public static bool SetCurrentProcessForeground()
        {
            var currentProcess = Process.GetCurrentProcess();
            var windowHandle = currentProcess.MainWindowHandle;
            if (windowHandle == IntPtr.Zero)
                return false;
            return NativeMethods.SetForegroundWindow(windowHandle);
        }
    }
}