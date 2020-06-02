using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundMixerSoftware.Common.Utils.Enum
{
    public static class EnumNameConverter
    {
        /// <summary>
        /// Get enum names defined my ValueNameAttribute
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetNames(Type enumType)
        {
            var names = System.Enum.GetNames(enumType);
            for (var n = 0; n < names.Length; n++)
            {
                yield return GetName(enumType, names[n]);
            }
        }

        public static T GetValue<T>(string name)
        {
            var valuesNames = GetNames(typeof(T)).ToList();
            var names = System.Enum.GetNames(typeof(T));
            return (T)System.Enum.Parse(typeof(T),names[valuesNames.IndexOf(name)]);
        }

        /// <summary>
        /// Get enum name defined my ValueNameAttribute
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static string GetName(Type enumType, string memberName)
        {
            var members = enumType.GetMember(memberName);
            for (var n = 0; n < members.Length; n++)
            {
                var member = members[n];
                if (Attribute.IsDefined(member, typeof(ValueNameAttribute)))
                {
                    var attribute = (ValueNameAttribute)Attribute.GetCustomAttribute(member, typeof(ValueNameAttribute));
                    return attribute.Name;
                }
            }
            return string.Empty;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ValueNameAttribute : Attribute
    {
        /// <summary>
        /// Defines name of enum`s field.
        /// </summary>
        public string Name { get; set; }

        public ValueNameAttribute(string name)
        {
            Name = name;
        }
    }
}