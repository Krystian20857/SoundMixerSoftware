using System;
using SoundMixerSoftware.Win32.Wrapper;

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
            return Array.Exists(ProcessWrapper.EnumProcesses(), u => u == pid);
        }
    }
}