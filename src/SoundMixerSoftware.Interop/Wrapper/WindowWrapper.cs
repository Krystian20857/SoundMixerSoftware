using System;
using System.Drawing;
using System.Text;
using NLog;
using SoundMixerSoftware.Interop.Constant;
using SoundMixerSoftware.Interop.Enum;
using SoundMixerSoftware.Interop.Method;

namespace SoundMixerSoftware.Interop.Wrapper
{
    /// <summary>
    /// Contains useful methods with windows.(bring to front, minimaze, close, etc)
    /// </summary>
    public class WindowWrapper
    {
        #region Current Class Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion

        /// <summary>
        /// Gets windows title.
        /// </summary>
        /// <param name="windowHandle">window handle.</param>
        /// <returns>string window title.</returns>
        public static string GetWindowTitle(IntPtr windowHandle)
        {
            var textLength = User32.GetWindowTextLength(windowHandle);
            var titleBuilder = new StringBuilder(textLength);
            User32.GetWindowText(windowHandle, titleBuilder, textLength + 1);
            return titleBuilder.ToString();
        }

        /// <summary>
        /// Set top layer window opacity.
        /// </summary>
        /// <param name="windowHandle">handle to window</param>
        /// <param name="alpha">byte alpha value</param>
        /// <returns></returns>
        public static bool SetWindowOpacity(IntPtr windowHandle, byte alpha)
        {
            return User32.SetLayeredWindowAttributes(windowHandle, 0, alpha, LWA.LWA_ALPHA);
        }

        /// <summary>
        /// Set window able to change opacity.
        /// </summary>
        /// <param name="windowHandle"></param>
        public static void ApplyOpacityFlag(IntPtr windowHandle)
        {
            User32.SetWindowLongPtr(windowHandle, (int)GWL.GWL_EXSTYLE, User32.GetWindowLongPtr(windowHandle, (int) GWL.GWL_EXSTYLE).ToInt32() ^ (int) GWL.GWL_EXSTYLE);
        }
        
        /// <summary>
        /// Get window icon.
        /// </summary>
        /// <param name="windowHandle">handle to window</param>
        /// <returns></returns>
        public static Icon GetWindowIcon(IntPtr windowHandle)
        {
            var iconHandle = IntPtr.Zero;
            try
            {
                iconHandle = User32.SendMessage(windowHandle, (int) WM.WM_GETICON, (IntPtr) ICON.ICON_BIG, IntPtr.Zero);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = User32.SendMessage(windowHandle, (int) WM.WM_GETICON, (IntPtr) ICON.ICON_SMALL, IntPtr.Zero);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = User32.SendMessage(windowHandle, (int) WM.WM_GETICON, (IntPtr) ICON.ICON_SMALL2, IntPtr.Zero);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = User32.GetClassLongPtr(windowHandle, GCL.GCL_HICON);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = User32.GetClassLongPtr(windowHandle, GCL.GCL_HICONSM);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                if (exception is OverflowException)
                    Logger.Error("Application might be running in 32-bit mode. If you are running 32-bit os there may occurs problems with getting 64-bit pointers.");
                return null;
            }

            if (iconHandle == IntPtr.Zero)
                return null;
            return Icon.FromHandle(iconHandle);
        }

        /// <summary>
        /// Get primary screen width.
        /// </summary>
        /// <returns></returns>
        public static int GetScreenWidth() => User32.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        
        /// <summary>
        /// Get primary screen height.
        /// </summary>
        /// <returns></returns>
        public static int GetScreenHeight() => User32.GetSystemMetrics(SystemMetric.SM_CYSCREEN);

        public static bool SetWindowPos(IntPtr handle, int x, int y)
        {
            return User32.SetWindowPos(handle, IntPtr.Zero, x, y, 0, 0, (uint)(SWP.NOSIZE | SWP.NOZORDER | SWP.SHOWWINDOW));
        }

    }
}