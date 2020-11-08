using System;
using System.Collections.ObjectModel;

namespace SoundMixerSoftware.Common.Extension
{
    public static class ListExtension
    {

        /// <summary>
        /// Linq any() using for-loop.
        /// </summary>
        /// <param name="collection">collection</param>
        /// <param name="predicate">condition</param>
        /// <typeparam name="TSource">value type</typeparam>
        /// <returns></returns>
        public static bool OptimizedAny<TSource>(this Collection<TSource> collection, Func<TSource, bool> predicate)
        {
            for(var n = 0; n < collection.Count; n++)
                if (predicate(collection[n]))
                    return true;
            return false;
        }
        
    }
}