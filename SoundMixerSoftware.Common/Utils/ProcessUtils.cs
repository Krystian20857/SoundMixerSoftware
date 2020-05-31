using System.Diagnostics;
using System.Linq;

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
            return Process.GetProcesses().Any(x => x.Id == pid);
        }
    }
}