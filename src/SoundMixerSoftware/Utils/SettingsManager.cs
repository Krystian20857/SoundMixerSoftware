using System;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Framework.Config;

namespace SoundMixerSoftware.Utils
{
    public static class SettingsManager
    {
        private static DebounceDispatcher _debounceDispatcher = new DebounceDispatcher();

        public static void HandleSPropertyChange<T>(string propertyName, bool lockConfig = false)
        {
            var property = typeof(T).GetProperty(propertyName ?? string.Empty);
            if (property == null)
                return;
            var attribute = (SettingPropertyAttribute) Attribute.GetCustomAttribute(property, typeof(SettingPropertyAttribute));
            if(attribute == null)
                return;
            if (!lockConfig || attribute.IgnoreConfigLock)
            {
                if (attribute.Debounce)
                    _debounceDispatcher.Debounce(300, param => ConfigHandler.SaveConfig());
                else
                    ConfigHandler.SaveConfig();
            }
        }
    }
    
    public class SettingPropertyAttribute : Attribute
    {
        public bool Debounce { get; set; }
        public bool IgnoreConfigLock { get; set; }

        public SettingPropertyAttribute(bool debounce = false, bool ignoreConfigLock = false)
        {
            Debounce = debounce;
            IgnoreConfigLock = ignoreConfigLock;
        }
    }
}