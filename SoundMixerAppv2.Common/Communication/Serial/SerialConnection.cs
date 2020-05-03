using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using NLog;

namespace SoundMixerAppv2.Common.Communication.Serial
{
    public class SerialConnection : IDisposable
    {
        #region Logger

        /// <summary>
        /// Current Class Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Contains current connected devices.
        /// </summary>
        public IReadOnlyList<string> ConnectedDevices => _connectedDevices.Keys.ToList().AsReadOnly();

        #endregion
        
        #region Events

        /// <summary>
        /// Fires when devices has connected.
        /// </summary>
        public event EventHandler<DeviceStateChangeArgs> OnDeviceConnected;
        /// <summary>
        /// Fires when devices has disconnected.
        /// </summary>
        public event EventHandler<DeviceStateChangeArgs> OnDeviceDisconnected;
        /// <summary>
        /// Fires when new data arrives.
        /// </summary>
        public event EventHandler<SerialDataReceivedArgs> OnDataReceived;

        #endregion
        
        #region Private Fields

        private readonly Dictionary<string, SerialPort> _connectedDevices = new Dictionary<string, SerialPort>();
        private readonly SerialConfig _serialConfig;

        #endregion

        #region Constructor

        public SerialConnection(SerialConfig serialConfig)
        {
            _serialConfig = serialConfig;
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Connect to serial device.
        /// </summary>
        /// <param name="comport">Serial device location.</param>
        /// <returns>Device connection state.</returns>
        public bool Connect(string comport)
        {
            if (_connectedDevices.ContainsKey(comport))
            {
                var device = _connectedDevices[comport];
                if (!device.IsOpen)
                    device.Open();
                if (device.IsOpen)
                    OnDeviceConnected?.Invoke(this, new DeviceStateChangeArgs(comport));
                return device.IsOpen;
            }
            else
            {
                var device = SerialConfig.SetupSerialPort(_serialConfig);
                device.PortName = comport;
                device.DataReceived += DeviceOnDataReceived;
                _connectedDevices.Add(comport, device);
                device.Open();
                if (device.IsOpen)
                {
                    OnDeviceConnected?.Invoke(this, new DeviceStateChangeArgs(comport));
                    Logger.Info($"Device connected: {comport}");
                }

                return device.IsOpen;
            }
        }

        /// <summary>
        /// Disconnect serial device.
        /// </summary>
        /// <param name="comport">Serial device location</param>
        /// <returns>Returns true when device disconnects.</returns>
        public bool Disconnect(string comport)
        {
            if (!_connectedDevices.ContainsKey(comport))
                return false;
            var device = _connectedDevices[comport];
            if (device.IsOpen)
                device.Close();
            if (!device.IsOpen)
            {
                OnDeviceDisconnected?.Invoke(this, new DeviceStateChangeArgs(comport));
                Logger.Info($"Device disconnected: {comport}");
            }
            return true;
        }
        
        #endregion
        
        #region Private Methods
        
        private void DeviceOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = sender as SerialPort;
            var data = new byte[serialPort.BytesToRead];
            serialPort.Read(data, 0, data.Length);
            var eventArgs = new SerialDataReceivedArgs(data, serialPort.PortName);
            OnDataReceived?.Invoke(this, eventArgs);
        }
        
        #endregion
        
        #region Dispose

        /// <summary>
        /// Dispose.....................
        /// </summary>
        public void Dispose()
        {
            foreach (var entry in _connectedDevices)
            {
                OnDeviceDisconnected?.Invoke(this, new DeviceStateChangeArgs(entry.Value.PortName));
                entry.Value.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        #endregion
        
    }
}