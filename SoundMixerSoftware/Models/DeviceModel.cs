﻿using SoundMixerSoftware.Common.Utils;
 using SoundMixerSoftware.Helpers;
 using SoundMixerSoftware.Win32.USBLib;

 namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Device model used in view
    /// </summary>
    public class DeviceModel
    {
        #region Public Properties

        /// <summary>
        /// Device Friendly name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Device ComPort.
        /// </summary>
        public string ComPort { get; set; }

        /// <summary>
        /// Device Vendor Id.
        /// </summary>
        public string Vid { get; set; }

        /// <summary>
        /// Device product Id.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Device count of sliders.
        /// </summary>
        public string Sliders { get; set; }

        /// <summary>
        /// Device count of buttons
        /// </summary>
        public string Buttons { get; set; }

        public string UUID { get; set; }

        #endregion
        
        #region Public Static Methods
        
        /// <summary>
        /// Create DeviceModel from DeviceProperties and DeviceIdResponse.
        /// </summary>
        /// <param name="properties">device properties</param>
        /// <param name="response"> device id response</param>
        /// <returns></returns>
        public static DeviceModel CreateModel(DeviceProperties properties, DeviceIdResponse response)
        {
            return new DeviceModel
            {
                Buttons = response.button_count.ToString(),
                ComPort = properties.COMPort,
                Name = response.name,
                Pid = "0x" + properties.Pid.ToString("X2"),
                Vid = "0x" + properties.Vid.ToString("X2"),
                Sliders = response.slider_count.ToString(),
                UUID = ArrayUtils.ConvertToString(response.uuid)
            };
        }
        
        #endregion
    }
}