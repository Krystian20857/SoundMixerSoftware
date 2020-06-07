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
        public static bool MergeObjects<T>(T source, T dest)
        {
            var modified = false;
            var type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                var valueSource = property.GetValue(source);
                var valueDest = property.GetValue(dest);
                if (valueSource == null)
                {
                    property.SetValue(source, valueDest);
                    modified = true;
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