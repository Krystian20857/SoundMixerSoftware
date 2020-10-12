using System.Windows.Media;

namespace SoundMixerSoftware.Common.Utils
{
    public static class ColorUtil
    {
        /// <summary>
        /// Convert ARGB int ot color.
        /// A: (8bits)
        /// R: (8bits)
        /// G: (8bits)
        /// B: (8bits)
        /// </summary>
        /// <param name="argb"></param>
        /// <returns></returns>
        public static Color FromArgb(int argb) => Color.FromArgb(
                (byte)((argb & 0xFF000000) >> 24),
                (byte)((argb & 0x00FF0000) >> 16),
                (byte)((argb & 0x0000FF00) >> 8),
                (byte)(argb & 0x000000FF));
    }
}