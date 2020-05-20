using System;
using System.Collections.Generic;
using System.Linq;
using SoundMixerSoftware.Helpers;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Win32.USBLib;

namespace SoundMixerSoftware.Models
{
    public static class DeviceHandlerGlobal
    {

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
        
        #endregion
        
        #region Static methods

        //...
        
        #endregion
    }
}