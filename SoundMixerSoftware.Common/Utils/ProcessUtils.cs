using System;
using SoundMixerSoftware.Win32.Interop.Enum;
using SoundMixerSoftware.Win32.Interop.Method;

namespace SoundMixerSoftware.Common.Utils
{
    public static class ProcessUtils
    {
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