using System;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace SoundMixerSoftware.Common.Cache
{
    public class NativeCache<T> : ICache<T>
    {
        #region Logger

        /// <summary>
        /// Current Class Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Private Fields

        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        #endregion
        
        #region Implemented  Methods

        public T GetOrCreate(object key, Func<T> createItem)
        {
            if (!_cache.TryGetValue(key, out T cacheEntry))
            {
                cacheEntry = createItem();
                _cache.Set(key, cacheEntry);
                Logger.Info($"Created cache entry: {key}");
            }

            return cacheEntry;
        }
        
        #endregion
    }
}