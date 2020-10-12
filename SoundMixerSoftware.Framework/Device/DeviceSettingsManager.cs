using SoundMixerSoftware.Helpers.Config;

namespace SoundMixerSoftware.Helpers.Device
{
    public static class DeviceSettingsManager
    {
        #region Public Methods

        /// <summary>
        /// Get device settings from config.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static DeviceSettings GetSettings(DeviceId deviceId)
        {
            var stringDeviceId = deviceId.ToString();
            var deviceStruct = ConfigHandler.ConfigStruct.Hardware.DeviceSettings;
            return deviceStruct.ContainsKey(stringDeviceId) ? deviceStruct[stringDeviceId] : new DeviceSettings();
        }

        /// <summary>
        /// Set device settings to config.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settings"></param>
        public static void SetSettings(DeviceId deviceId, DeviceSettings settings)
        {
            var stringDeviceId = deviceId.ToString();
            var deviceStruct = ConfigHandler.ConfigStruct.Hardware.DeviceSettings;
            if (deviceStruct.ContainsKey(stringDeviceId))
                ConfigHandler.ConfigStruct.Hardware.DeviceSettings[stringDeviceId] = settings;
            else
                ConfigHandler.ConfigStruct.Hardware.DeviceSettings.Add(stringDeviceId, settings);
        }
        
        #endregion
    }
}