using System.Collections;

namespace SoundMixerAppv2.Common.Extension
{
    public static class ICollectionExtension
    {
        /// <summary>
        /// Convert collection content to object array.
        /// </summary>
        /// <returns>Converted collection.</returns>
        public static object[] ToArray(this ICollection iCollection)
        {
            return ToArray<object>(iCollection);
        }
        
        /// <summary>
        /// Convert collection content to array.
        /// </summary>
        /// <returns>Converted collection.</returns>
        public static T[] ToArray<T>(this ICollection iCollection)
        {
            var array = new T[iCollection.Count];
            iCollection.CopyTo(array, 0);
            return array;
        }
    }
}