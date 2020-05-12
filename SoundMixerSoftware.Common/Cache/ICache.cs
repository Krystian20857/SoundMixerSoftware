using System;

namespace SoundMixerSoftware.Common.Cache
{
    public interface ICache<T>
    {
        T GetOrCreate(object key, Func<T> createItem);
    }
}