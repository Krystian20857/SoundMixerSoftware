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

        private static List<(DeviceProperties properties, DeviceIdResponse device)> _connectedDevices = new List<(DeviceProperties properties, DeviceIdResponse device)>();

        #endregion
        
        #region Public Static Fields

        /// <summary>
        /// Global Device Handler.
        /// </summary>
        public static DeviceHandler DeviceHandler { get; }
        /// <summary>
        /// Currently connected devices.
        /// </summary>
        public static IReadOnlyList<(DeviceProperties properties, DeviceIdResponse device)> ConnectedDevice => _connectedDevices.AsReadOnly();

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
            _connectedDevices.Add((e.Device, e.DeviceResponse));
        }
        
        private static void DeviceHandlerOnDeviceDisconnected(object sender, DeviceStateArgs e)
        {
            var toRemove = _connectedDevices.FirstOrDefault(x => x.properties.COMPort.Equals(e.DeviceProperties.COMPort, StringComparison.InvariantCultureIgnoreCase));
            _connectedDevices.Remove(toRemove);
        }
        
        #endregion
        
        #region Static methods

        //...
        
        #endregion
    }
}