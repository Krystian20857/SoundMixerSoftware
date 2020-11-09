using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundMixerSoftware.Interop.Method;

namespace SoundMixerSoftware.Common.Extension
{
    public static class BitmapExtension
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
            if(!Gdi32.DeleteObject(ptr))
                throw new Win32Exception();
            return result;
        }

        /// <summary>
        /// Convert Bitmap to Icon.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Icon ToIcon(this Bitmap bitmap)
        {
            return Icon.FromHandle(bitmap.GetHicon());
        }
        
        /// <summary>
        /// Convert bitmap to byte array.
        /// </summary>
        /// <param name="bitmap">Bitmap.</param>
        /// <returns>Byte Array.</returns>
        public static byte[] ToByteArray(this Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

    }
}