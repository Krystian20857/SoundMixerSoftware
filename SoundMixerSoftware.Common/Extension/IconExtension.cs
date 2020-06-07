using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundMixerSoftware.Win32.Win32;

namespace SoundMixerSoftware.Common.Extension
{
    public static class IconExtension
    {
        /// <summary>
        /// Convert Bitmap to ImageSource
        /// </summary>
        /// <param name="icon"></param>
        /// <returns>ImageSource</returns>
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            var ptr = bitmap.GetHbitmap();
            var result = Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            if(!NativeMethods.DeleteObject(ptr))
                throw new Win32Exception();
            return result;
        }

        /// <summary>
        /// Convert Icon to ImageSource
        /// </summary>
        /// <param name="icon"></param>
        /// <returns>ImageSource</returns>
        /// <exception cref="Win32Exception"></exception>
        public static ImageSource ToImageSource(this Icon icon)
        {
            var handle = icon.Handle;
            var result = Imaging.CreateBitmapSourceFromHIcon(handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return result;
        }

        /// <summary>
        /// Convert icon instance to ByteArray.
        /// </summary>
        /// <param name="icon">Icon to convert</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Icon icon)
        {
            var bitmap = icon.ToBitmap();
            if (bitmap == null)
                return new byte[0];
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}