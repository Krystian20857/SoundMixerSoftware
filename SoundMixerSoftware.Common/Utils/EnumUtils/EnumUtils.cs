using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Common.Utils.EnumUtils
{
    public static class EnumUtils
    {
        /// <summary>
        /// Parse enum value from string.
        /// </summary>
        /// <param name="input">input object</param>
        /// <typeparam name="T">enum type</typeparam>
        /// <returns></returns>
        public static T Parse<T>(object input) where T : struct
        {
            if(!typeof(T).IsEnum)
                throw new ArgumentException("Type must be enum.");
            return Enum.TryParse<T>(input.ToString(), out var result) ? result : default;
        }

        /// <summary>
        /// Parse enum value enumerable type to from object enumerable type.
        /// </summary>
        /// <param name="input">input enumerable type</param>
        /// <typeparam name="T">enum type</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Parse<T>(IEnumerable<object> input) where T : struct
        {
            foreach (var enumValue in input)
                yield return Parse<T>(enumValue);
        }
        
    }
}