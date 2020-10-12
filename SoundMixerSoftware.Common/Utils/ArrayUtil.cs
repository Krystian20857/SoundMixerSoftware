using System;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace SoundMixerSoftware.Common.Utils
{
    /// <summary>
    /// Contains methods that helps with array handling
    /// </summary>
    public static class ArrayUtil
    {
        /// <summary>
        /// Convert valueType array to hex string terminated with separator
        /// </summary>
        /// <param name="array"></param>
        /// <param name="separator"></param>
        /// <param name="isUpperCase"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ConvertToHexString<T>(T[] array, string separator = "-", bool isUpperCase = true) where T : struct, IFormattable
        {
            var builder = new StringBuilder();
            for (var n = 0; n < array.Length; n++)
            {
                var stringHex = array[n].ToString("X2", new NumberFormatInfo());
                builder.Append(isUpperCase ? stringHex.ToUpper( ): stringHex.ToLower());
                if (n != array.Length - 1)
                    builder.Append('-');
            }
            return builder.ToString();
        }
        
    }
}