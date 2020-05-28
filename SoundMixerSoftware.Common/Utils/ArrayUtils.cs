using System;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace SoundMixerSoftware.Common.Utils
{
    /// <summary>
    /// Contains methods that helps with array handling
    /// </summary>
    public static class ArrayUtils
    {

        /// <summary>
        /// Convert valueType array to hex string terminated with '-'
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ConvertToString<T>(T[] array) where T : struct, IFormattable
        {
            var builder = new StringBuilder();
            for (var n = 0; n < array.Length; n++)
            {
                builder.Append(array[n].ToString("X2", new NumberFormatInfo()));
                if (n != array.Length - 1)
                    builder.Append('-');
            }
            return builder.ToString();
        }
        
    }
}