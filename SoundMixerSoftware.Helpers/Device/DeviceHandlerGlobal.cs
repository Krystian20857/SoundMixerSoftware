using System.Collections.Generic;
using System.Diagnostics;
using NLog;
using SoundMixerSoftware.Helpers;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Win32.USBLib;
using DataReceivedEventArgs = SoundMixerSoftware.Common.Communication.DataReceivedEventArgs;

namespace SoundMixerSoftware.Models
{
    public static class DeviceHandlerGlobal
    {
        #region Current Class Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Static Fields

        private static Dictionary<string, DeviceConnectedEventArgs> _connectedDevices = new Dictionary<string, DeviceConnectedEventArgs>();

        #endregion
        
        #region Public Static Fields

        /// <summary>
        /// Global Device Handler.
        /// </summary>
        public static DeviceHandler DeviceHandler { get; }

        /// <summary>
        /// Currently connected devices.
        /// </summary>
        public static IReadOnlyDictionary<string, DeviceConnectedEventArgs> ConnectedDevice => _connectedDevices;

        #endregion
        
        #region constructor

        static DeviceHandlerGlobal()
        {
            DeviceHandler = new DeviceHandler();
            DeviceHandler.DeviceConnected += DeviceHandlerOnDeviceConnected;
            DeviceHandler.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;
            DeviceHandler.DataReceived += DeviceHandlerOnDataReceived;
            
            RegisterTypes();
        }

        #endregion
        
        #region Events

        private static void DeviceHandlerOnDeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            _connectedDevices.Add(e.Device.COMPort, e);
        }
        
        private static void DeviceHandlerOnDeviceDisconnected(object sender, DeviceStateArgs e)
        {
            _connectedDevices.Remove(e.DeviceProperties.COMPort);
        }
        
        private static void DeviceHandlerOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            switch (e.Command)
            {
                case 0x01:
                    SliderStruct sliderStruct = e.Data;
                    var index = sliderStruct.slider;
                    if (index >= SessionHandler.Sliders.Count)
                    {
                        Logger.Warn("Slider receive index mismatch.");
                        return;
                    }
                    SessionHandler.SetVolume(index, sliderStruct.value / 100.0F, false);
                    break;
            }
        }
        
        #endregion
        
        #region Static methods

        private static void RegisterTypes()
        {
            DeviceHandler.RegisterType(0x01, typeof(SliderStruct));
        }
        
        #endregion
    }
}