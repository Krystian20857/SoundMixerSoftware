using System;
using System.Text;
using SoundMixerSoftware.Win32.Interop.Enum;
using SoundMixerSoftware.Win32.Interop.Method;

namespace SoundMixerSoftware.Common.Utils
{
    public static class ProcessUtils
    {
        /// <summary>
        /// Get Process file name by standard WIN32 method
        /// </summary>
        /// <param name="process"></param>
        /// <param name="buffer">buffer size default: 260</param>
        /// <returns></returns>
        public static string GetFileName(int pid, int buffer = 260) //260 -> max windows path length
        {
            var handle = Kernel32.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, pid);
            if (handle == IntPtr.Zero)
                return string.Empty;
            var nameBuilder = new StringBuilder(buffer);
            var bufferLength = nameBuilder.Capacity + 1;
            return Kernel32.QueryFullProcessImageName(handle, 0, nameBuilder, ref bufferLength) ? nameBuilder.ToString() : string.Empty;
        }
        
        /// <summary>
        /// Check if process id exists. Useful with zombie audio sessions war :).
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns></returns>
        public static bool IsAlive(int pid)
        {
            var phandle = Kernel32.OpenProcess(ProcessAccessFlags.Synchronize, false, pid);
            if (phandle == IntPtr.Zero)
                return false;
            return Kernel32.WaitForSingleObject(phandle, 0) != 0;
        }
    }
}