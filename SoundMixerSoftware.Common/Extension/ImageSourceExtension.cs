using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;

namespace SoundMixerSoftware.Common.Extension
{
    public static class ImageSourceExtension
    {
        public static Bitmap ToBitmap(this ImageSource imageSource)
        {
            var source = imageSource.ThrowIfNotBitmapSource();

            var bitmap = new Bitmap(source.PixelWidth, source.PixelWidth, PixelFormat.Format32bppPArgb);
            var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        public static Icon ToIcon(this ImageSource imageSource)
        {
            return imageSource.ToBitmap().ToIcon();
        }

        public static byte[] ToByteArray(this ImageSource imageSource, BitmapEncoder encoder = null)
        {
            var source = imageSource.ThrowIfNotBitmapSource();
            encoder = encoder ?? new PngBitmapEncoder();
            
            using (var memoryStream = new MemoryStream())
            {
                var frame = BitmapFrame.Create(source);
                encoder.Frames.Add(frame);
                encoder.Save(memoryStream);

                return memoryStream.ToArray();
            }
        }

        private static BitmapSource ThrowIfNotBitmapSource(this ImageSource source)
        {
            if(source is BitmapSource bitmapSource) 
                return bitmapSource;
            throw new ArgumentException($"ImageSource must be instance of {nameof(BitmapImage)}");
        }
        
    }
}