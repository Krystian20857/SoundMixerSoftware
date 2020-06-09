using System.Diagnostics;
using System.IO;

namespace SoundMixerSoftware.Common.Utils.Application
{
    /// <summary>
    /// Contains useful methods.
    /// </summary>
    public static class AppUtils
    {
        /// <summary>
        /// Open windows explorer in specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool OpenExplorer(string directory)
        {
            if (!Directory.Exists(directory))
                return false;
            Process.Start("explorer.exe", directory);
            return true;
        }
        
    }
}