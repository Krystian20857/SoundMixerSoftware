using System;

namespace SoundMixerSoftware.Common.Utils
{
    /// <summary>
    /// Contains useful methods while working with objects and classes.
    /// </summary>
    public static class ObjectUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="dest">Destination object.</param>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>Returns true when any changes where made.</returns>
        public static bool MergeObjects<T>(ref T source, T dest)
        {
            var modified = false;
            if (source == null)
            {
                source = dest;
                modified = true;
            }

            var type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                if (source == null)
                    continue;
                var valueSource = property.GetValue(source);
                var valueDest = property.GetValue(dest);
                if (valueSource == null)
                {
                    property.SetValue(source, valueDest);
                    modified = true;
                }

                if (!property.PropertyType.IsValueType && Attribute.IsDefined(property, typeof(RecursionAttribute)))
                    modified |= InvokeGenericMethod<bool>(
                        typeof(ObjectUtil),
                        null,
                        nameof(MergeObjects),
                        property.PropertyType,
                        new[] {valueSource, valueDest});

            }

            return modified;
        }

        /// <summary>
        /// Invoke method with dynamic generic type.
        /// </summary>
        /// <param name="methodType">Type of method container(class, interface, etc.).</param>
        /// <param name="methodName">Name of method.</param>
        /// <param name="genericType">Dynamic generic type.</param>
        /// <param name="context">Invoke context of method.</param>
        /// <param name="parameters">Parameters to pass.</param>
        /// <typeparam name="T">Return type.</typeparam>
        /// <returns>Returns method result.</returns>
        /// 
        public static T InvokeGenericMethod<T>(Type methodType, object context, string methodName, Type genericType, object[] parameters)
        {
            var method = methodType.GetMethod(methodName);
            var generic = method.MakeGenericMethod(genericType);
            return (T) generic.Invoke(context, parameters);
        }
    }
}