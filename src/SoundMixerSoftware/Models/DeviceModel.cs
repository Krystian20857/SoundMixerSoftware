using SoundMixerSoftware.Framework.Device;
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
        public uint Vid { get; set; }

        /// <summary>
        /// Device product Id.
        /// </summary>
        public uint Pid { get; set; }

        /// <summary>
        /// Device count of sliders.
        /// </summary>
        public int Sliders { get; set; }

        /// <summary>
        /// Device count of buttons
        /// </summary>
        public int Buttons { get; set; }
        /// <summary>
        /// UUID of current device
        /// </summary>
        public DeviceId UUID { get; set; }

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
                Buttons = response.button_count,
                ComPort = properties.COMPort,
                Name = response.name,
                Pid =  properties.Pid,
                Vid =  properties.Vid,
                Sliders = response.slider_count,
                UUID = new DeviceId(response.uuid)
            };
        }
        
        /// <summary>
        /// Create DeviceModel from DeviceConnectedEventArgs.
        /// </summary>
        /// <param name="eventArgs">Event args</param>
        /// <returns></returns>
        public static DeviceModel CreateModel(DeviceConnectedEventArgs eventArgs)
        {
            return CreateModel(eventArgs.Device, eventArgs.DeviceResponse);
        }
        
        #endregion
    }
}