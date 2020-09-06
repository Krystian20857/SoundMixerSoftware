using System;
using System.Collections.Generic;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.Utils
{
    public static class EnumDisplayHelper
    {
        
        /// <summary>
        /// Add items with name to collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddItems<T>(ICollection<EnumDisplayModel<T>> collection) where T: struct, IConvertible
        {
            var enumNames = Enum.GetNames(typeof(T));
            foreach (var enumName in enumNames)
            {
                var mediaModel = new EnumDisplayModel<T>()
                {
                    EnumValue = (T) Enum.Parse(typeof(T), enumName)
                };
                collection.Add(mediaModel);
            }
        }
        
    }
}