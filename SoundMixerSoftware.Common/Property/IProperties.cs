using System.Collections.Generic;

namespace SoundMixerSoftware.Common.Property
{
    /// <summary>
    /// Allows type to store properties~.
    /// </summary>
    public interface IProperties<T,U>
    {
        /// <summary>
        /// Get value with key.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        U GetValue(T key);
        /// <summary>
        /// Sets value with specified key.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        void SetValue(T key, U value);
        /// <summary>
        /// Remove value/key pair.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>True when removal successfully </returns>
        bool RemoveValue(T key);
        /// <summary>
        /// Get all values.
        /// </summary>
        /// <returns></returns>
        IEnumerable<U> GetValues();
        /// <summary>
        /// Get all keys.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetKeys();
    }
}