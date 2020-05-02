using System;

namespace SoundMixerAppv2.Common.LocalSystem
{
    /// <summary>
    /// Can be use to store values in registry.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets and Sets key in registry use to get value.
        /// </summary>
        public string PropertyName { get; set;}

        /// <summary>
        /// Creates object use to store values in registry.
        /// </summary>
        /// <param name="propertyName">Sets key name in registry.</param>
        public PropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}