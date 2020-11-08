using System.Collections.Generic;

namespace SoundMixerSoftware.Common.Property
{
    public interface IContainer<T, U> : IEnumerable<KeyValuePair<T, U>>
    {
        /// <summary>
        /// Add entry to container.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        bool Add(T key, U value);
        /// <summary>
        /// Get value of key.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        U Get(T key);
        /// <summary>
        /// Get value of key adding new when not exists.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="defaultValue">default value of newly created entry.</param>
        /// <param name="modified">true when container has been modified.</param>
        /// <returns></returns>
        U Get(T key, U defaultValue, out bool modified); 
        /// <summary>
        /// Remove value with key.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        bool Remove(T key);
        /// <summary>
        /// Get Real Container.
        /// </summary>
        Dictionary<T, U> RealContainer { get; set; }
    }
}