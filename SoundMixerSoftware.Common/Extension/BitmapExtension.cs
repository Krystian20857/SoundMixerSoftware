using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SoundMixerSoftware.Common.Extension
{
    public static class BitmapExtension
    {

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