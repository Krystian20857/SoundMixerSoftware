using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using SoundMixerSoftware.Win32.Win32;
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
        public static string GetFileName(this Process process, int buffer = 1024)
        {
            var nameBuilder = new StringBuilder(buffer);
            var bufferLength = nameBuilder.Capacity + 1;
            return NativeMethods.QueryFullProcessImageName(process.Handle, 0, nameBuilder, ref bufferLength) ? nameBuilder.ToString() : null;
        }

        /// <summary>
        /// Get icon of current process.
        /// </summary>
        /// <param name="process"></param>
        /// <returns>System Drawing Icon</returns>
        public static Icon GetIcon(this Process process)
        {
            return Icon.ExtractAssociatedIcon(process.GetFileName());
        }
        
        /// <summary>
        /// Get main window icon of process. If process has not main window method returns icon of process file.
        /// </summary>
        /// <param name="process">target process.</param>
        /// <returns>icon</returns>
        public static Icon GetMainWindowIcon(this Process process)
        {
            var handle = FindSimilar(process).FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero)?.MainWindowHandle ?? IntPtr.Zero;
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
            var label = string.Empty;
            try
            {
                label = FileVersionInfo.GetVersionInfo(process.GetFileName()).FileDescription;
            }
            catch (Win32Exception win32Exception)
            {
                //Win32 hresult for access denied: dec: -2147467259 hex: 0x80004005
                if (win32Exception.ErrorCode == 0x80004005)
                    label = process.ProcessName;
            }
            if (!string.IsNullOrWhiteSpace(label)) return label;
            var handle = FindSimilar(process).FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero)?.MainWindowHandle ?? IntPtr.Zero;
            return handle == IntPtr.Zero ? null : WindowWrapper.GetWindowTitle(handle);
        }

        /// <summary>
        /// Find processes with same name.
        /// </summary>
        /// <param name="process">target process.</param>
        /// <returns>List of matched processes.</returns>
        public static IEnumerable<Process> FindSimilar(this Process process) => Process.GetProcesses().Where(x => x.ProcessName.Equals(process.ProcessName));
        
    }
}