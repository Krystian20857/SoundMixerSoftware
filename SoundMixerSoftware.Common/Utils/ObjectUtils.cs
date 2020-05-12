using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundMixerSoftware.Common.Utils
{
    /// <summary>
    /// Contains useful methods while working with objects and classes.
    /// </summary>
    public static class ObjectUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="dest">Destination object.</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Returns true when any changes where made.</returns>
        public static bool MergeObjects<T>(T source, T dest, List<Type> ignoredTypes)
        {
            var modified = false;
            var type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;
                if(ignoredTypes.Contains(propertyType))
                    continue;
                
                var propertyValue = property.GetValue(source);
                var propertyName = property.Name;
                var destPropertyValue = property.GetValue(dest);
                
                if(propertyType == typeof(Type))
                    MergeObjects((T)propertyValue, (T)type.GetProperty(propertyName).GetValue(dest), ignoredTypes);
                else if(HasEmptyConstructor(propertyType) || propertyType.IsValueType)
                {
                    if(destPropertyValue != null)
                        if (destPropertyValue.Equals(Activator.CreateInstance(propertyType)))
                        {
                            property.SetValue(dest, propertyValue);
                            modified = true;
                        }
                }
                else if (propertyType == typeof(string))
                {
                    if (destPropertyValue == null)
                    {
                        property.SetValue(dest, propertyValue);
                        modified = true;
                    }
                }
            }

            return modified;
        }

        /// <summary>
        /// Checks if the <see cref="type"/> has empty constructor.
        /// </summary>
        /// <param name="type">Checking type.</param>
        /// <returns>Returns true when has empty constructor.</returns>
        public static bool HasEmptyConstructor(Type type)
        {
            return type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0);
        }
    }
}