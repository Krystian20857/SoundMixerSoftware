using System.Collections.Generic;
using SoundMixerSoftware.Common.Config;
using SoundMixerSoftware.Common.Config.Yaml;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.LocalSystem;
using SoundMixerSoftware.Interop.USBLib;
using YamlDotNet.Serialization;

namespace SoundMixerSoftware.Framework.Device
{
    public static class DeviceSettingsManager
    {
        #region Fields
        
        private static IConfig<DeviceConfigStruct> _config;
        
        #endregion

        #region Methods

        /// <summary>
        /// Initialize device settings.
        /// </summary>
        public static void Initialize()
        {
            _config = new YamlConfig<DeviceConfigStruct>(LocalContainer.DeviceConfig);
        }

        /// <summary>
        /// Save settings to disk.
        /// </summary>
        public static void Save() => _config.SaveConfig();
        /// <summary>
        /// Load settings from disk.
        /// </summary>
        public static void Load() => _config.LoadConfig();

        public static DeviceConfigStruct AllSettings => _config.Config;

        /// <summary>
        /// Get device settings from config.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static DeviceSettings GetSettings(DeviceId deviceId)
        {
            var deviceIdString = deviceId.ToString();
            var deviceSettings = _config.Config.DeviceSettings;
            return deviceSettings.ContainsKey(deviceIdString) ? deviceSettings[deviceIdString] : new DeviceSettings();
        }

        /// <summary>
        /// Set device settings to config.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settings"></param>
        public static void SetSettings(DeviceId deviceId, DeviceSettings settings)
        {
            var deviceIdString = deviceId.ToString();
            var deviceSettings = _config.Config.DeviceSettings;
            if (deviceSettings.ContainsKey(deviceIdString))
                deviceSettings[deviceIdString] = settings;
            else
                deviceSettings.Add(deviceIdString, settings);
        }
        
        #endregion
    }

    public class DeviceConfigStruct : IConfigStruct<DeviceConfigStruct>
    {
        #region Implemented Things
        
        public static DeviceConfigStruct SampleConfig { get; } = new DeviceConfigStruct
        {
            DeviceSettings = new Dictionary<string, DeviceSettings>(),
            UsbIDs = new List<USBID>()
            {
                new USBID{ Vid = 0x468F, Pid = 0x895D}
            },
        };

        public DeviceConfigStruct GetSampleConfig() => SampleConfig;

        public object Clone()
        {
            return MemberwiseClone();
        }
        
        #endregion
        
        #region Public Properties

        [YamlMember(Alias = "Devices")]
        public Dictionary<string, DeviceSettings> DeviceSettings { get; set; }

        public List<USBID> UsbIDs { get; set; }

        #endregion
    }
}