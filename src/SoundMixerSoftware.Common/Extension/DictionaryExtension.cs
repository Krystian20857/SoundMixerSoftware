using System.Collections.Generic;

namespace SoundMixerSoftware.Common.Extension
{
    public static class DictionaryExtension
    {

        public static bool Set<T, U>(this Dictionary<T, U> dictionary, T key, U value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return false;
            }
            dictionary.Add(key, value);
            return true;
        }
        
    }
}