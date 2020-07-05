using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        /// <summary>
        /// Gets windows title.
        /// </summary>
        /// <param name="windowHandle">window handle.</param>
        /// <returns>string window title.</returns>
        public static string GetWindowTitle(IntPtr windowHandle)
        {
            var textLength = NativeMethods.GetWindowTextLength(windowHandle);
            var titleBuilder = new StringBuilder(textLength);
            NativeMethods.GetWindowText(windowHandle, titleBuilder, textLength + 1);
            return titleBuilder.ToString();
        }

        public static Icon GetWindowIcon(IntPtr windowHandle)
        {
            var iconHandle = IntPtr.Zero;
            try
            {
                iconHandle = NativeMethods.SendMessage(windowHandle, (int) NativeClasses.WM.WM_GETICON, (IntPtr) NativeClasses.ICON.ICON_BIG, IntPtr.Zero);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = NativeMethods.SendMessage(windowHandle, (int) NativeClasses.WM.WM_GETICON, (IntPtr) NativeClasses.ICON.ICON_SMALL, IntPtr.Zero);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = NativeMethods.SendMessage(windowHandle, (int) NativeClasses.WM.WM_GETICON, (IntPtr) NativeClasses.ICON.ICON_SMALL2, IntPtr.Zero);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = NativeMethods.GetClassLongPtr(windowHandle, NativeClasses.GCL.GCL_HICON);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = NativeMethods.GetClassLongPtr(windowHandle, NativeClasses.GCL.GCL_HICONSM);
            }
            catch (Exception)
            {
                return null;
            }

            if (iconHandle == IntPtr.Zero)
                return null;
            return Icon.FromHandle(iconHandle);
        }

        //public static IEnumerable<IntPtr> GetWindowHandles(int processId)
        
    }
}