using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Win32.Interop;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Common.Extension
{
    public static class ProcessExtension
    {
        /// <summary>
        /// Get Process file name by standard WIN32 method
        /// </summary>
        /// <param name="process"></param>
        /// <param name="buffer">buffer size default: 1024</param>
        /// <returns></returns>
        public static string GetFileName(this Process process, int buffer = 260) //260 -> max windows path length
        {
            return ProcessWrapper.GetFileName(process.Id, buffer);
        }

        /// <summary>
        /// Get icon of current process.
        /// </summary>
        /// <param name="process"></param>
        /// <returns>System Drawing Icon</returns>
        public static Icon GetIcon(this Process process)
        {
            var processFile = process.GetFileName();
            if (!File.Exists(processFile))
                return null;
            return Icon.ExtractAssociatedIcon(processFile);
        }
        
        /// <summary>
        /// Get main window icon of process. If process has not main window method returns icon of process file.
        /// </summary>
        /// <param name="process">target process.</param>
        /// <returns>icon</returns>
        public static Icon GetMainWindowIcon(this Process process)
        {
            var handle = process.MainWindowHandle;
            if (handle == IntPtr.Zero)
                return GetIcon(process);
            return WindowWrapper.GetWindowIcon(handle);
        }

        /// <summary>
        /// Gets string describing process default returns description from process file. If file does not contains description method will return main window title.
        /// </summary>
        /// <param name="process">target process.</param>
        /// <returns>string description.</returns>
        public static string GetPreciseName(this Process process)
        {
            /*Order:
             *  - name from apps folder
             *  - main window title
             *  - exe file description
             *  - exe file name
             */
            var displayName = AppWrapper.GetAppName((uint) process.Id);
            if (!string.IsNullOrEmpty(displayName)) return displayName;

            try
            {
                var handle = process.MainWindowHandle;
                if (handle != IntPtr.Zero) return WindowWrapper.GetWindowTitle(handle);
            }
            catch (Exception ex)
            {
                
            }

            try
            {
                displayName = FileVersionInfo.GetVersionInfo(process.GetFileName()).FileDescription;
            }
            catch (Exception exception)
            {
                if(exception is Win32Exception win32exception)
                    if ((uint)win32exception.ErrorCode == 0x80004005)
                        return process.ProcessName;
            }

            return string.IsNullOrEmpty(displayName) ? process.ProcessName : displayName;
        }

    }
}