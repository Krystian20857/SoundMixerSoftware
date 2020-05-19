using System.Diagnostics;
using System.Drawing;
using System.Text;
using SoundMixerSoftware.Win32.Win32;

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
    }
}