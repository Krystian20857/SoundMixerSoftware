using System;
using System.Drawing;
using System.Windows.Forms;
using SoundMixerSoftware.Win32.Win32;

namespace SoundMixerSoftware.Win32.Wrapper
{
    public static class IconExtractor
    {
        
        /// <summary>
        /// Get int pointer of image in specified file;
        /// </summary>
        /// <param name="file">Absolute file path.</param>
        /// <param name="index">Index of icon.</param>
        /// <param name="largeIcon">true: Extract large icon. false: Extract small icon.</param>
        /// <returns>Int pointer.</returns>
        public static IntPtr ExtractPtr(string file, int index, bool largeIcon)
        {
            NativeMethods.ExtractIconEx(file, index, out var large, out var small,1);
            try
            {
                return largeIcon ? large : small;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Extract icon from file to Icon instance.
        /// </summary>
        /// <param name="file">Absolute file path.</param>
        /// <param name="index">Index of icon.</param>
        /// <param name="largeIcon">true: Extract large icon. false: Extract small icon.</param>
        /// <returns>Icon.</returns>
        public static Icon ExtractIcon(string file, int index, bool largeIcon)
        {
            var ptr = ExtractPtr(file, index, largeIcon);
            if (ptr == IntPtr.Zero)
                return null;
            return Icon.FromHandle(ptr);
        }

        /// <summary>
        /// Extract icon from file to another file.
        /// </summary>
        /// <param name="file">Absolute input file path.</param>
        /// <param name="index">Index of icon.</param>
        /// <param name="largeIcon">true: Extract large icon. false: Extract small icon.</param>
        /// <param name="fileToExtract">Absolute output file path</param>
        public static void ExtractToFile(string file, int index, bool largeIcon, string fileToExtract)
        {
            ExtractIcon(file, index, largeIcon)?.ToBitmap().Save(fileToExtract);
        }

        /// <summary>
        /// Extract icon using FormatIconPath.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Icon ExtractFromIndex(string path)
        {
            var data = FormatIconPath(path);
            return ExtractIcon(data.Path, data.Index, true);
        }
        
        /// <summary>
        /// Format icon string to path and index.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static (string Path, int Index) FormatIconPath(string path)
        {
            var data = path.Split(',');
            return (data[0], int.Parse(data[1]));
        }
    }
    
}