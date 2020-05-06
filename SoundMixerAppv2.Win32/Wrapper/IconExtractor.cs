using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using SoundMixerAppv2.Win32.Win32;

namespace SoundMixerAppv2.Win32.Wrapper
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
            /*Creating Sharp Edged
            using (var iconStream  = new FileStream(fileToExtract + ".ico", FileMode.Create))
                ExtractIcon(file, index, largeIcon)?.Save(iconStream);*/
        }
    }
    
}