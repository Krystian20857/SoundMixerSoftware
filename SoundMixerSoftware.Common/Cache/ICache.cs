using System;

namespace SoundMixerAppv2.Common.Cache
{
    public interface ICache<T>
    {
        T GetOrCreate(object key, Func<T> createItem);
    }
}