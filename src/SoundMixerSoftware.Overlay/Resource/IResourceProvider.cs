using System.Collections.Generic;

namespace SoundMixerSoftware.Overlay.Resource
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type of output resource.</typeparam>
    public interface IResourceProvider<T>
    {
        /// <summary>
        /// Get resource with specified key.
        /// </summary>
        /// <param name="resourceKey">Key to resource.</param>
        /// <returns>Resource object.</returns>
        T GetResource(string resourceKey);
        /// <summary>
        /// Set resource with specified key.
        /// </summary>
        /// <param name="resourceKey">Resource key.</param>
        /// <param name="resource">Resource object.</param>
        void SetResource(string resourceKey, T resource);
        /// <summary>
        /// Gets and gets resource by array operator.
        /// </summary>
        /// <param name="resourceKey"></param>
        T this[string resourceKey] { get; set; }
        /// <summary>
        /// Get all resources.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, T>> GetResources();
    }
}