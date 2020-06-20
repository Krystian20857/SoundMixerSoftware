using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SoundMixerSoftware.Common.Utils;

namespace SoundMixerSoftware.Common.Communication.Serial
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
        public event EventHandler<DeviceStateChangeArgs> DeviceConnected;
        /// <summary>
        /// Fires when device not responding for specified time 
        /// </summary>
        public event EventHandler<DeviceStateChangeArgs> DeviceDisconnected;
        /// <summary>
        /// Fires when new data arrives.
        /// </summary>
        public event EventHandler<SerialDataReceivedArgs> DataReceived;

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
        public bool Connect(string comport, bool fireEvent = true)
        {
            try
            {
                if (!SerialPort.GetPortNames().Contains(comport))
                    return false;
                if (_connectedDevices.ContainsKey(comport))
                {
                    var device = _connectedDevices[comport];
                    if (!device.IsOpen)
                        device.Open();
                    if (device.IsOpen && fireEvent)
                        DeviceConnected?.Invoke(this, new DeviceStateChangeArgs(comport));
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
                        if(fireEvent)
                            DeviceConnected?.Invoke(this, new DeviceStateChangeArgs(comport));
                        Logger.Info($"Device connected: {comport}");
                    }

                    return device.IsOpen;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Disconnect serial device.
        /// </summary>
        /// <param name="comport">Serial device location.</param>
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
                DeviceDisconnected?.Invoke(this, new DeviceStateChangeArgs(comport));
                Logger.Info($"Device disconnected: {comport}");
            }
            return true;
        }

        /// <summary>
        /// Send struct by serial connection
        /// </summary>
        /// <param name="comport">Serial device location.</param>
        /// <param name="structure">Data to send.</param>
        /// <typeparam name="T">Type of struct.</typeparam>
        public void SendData<T>(string comport, T structure) where T : struct
        {
            var data = StructUtils.StructToBytes<T>(structure);
            SendBytes(comport, data);
        }

        /// <summary>
        /// Send bytes array using serial.
        /// </summary>
        /// <param name="comport">Serial port location.</param>
        /// <param name="bytes">Byte array.</param>
        public void SendBytes(string comport, byte[] bytes)
        {
            if (_connectedDevices.ContainsKey(comport))
            {
                var device = _connectedDevices[comport];
                if (device.IsOpen)
                {
                    device.Write(bytes, 0, bytes.Length);
                }
                else
                    Logger.Warn($"Device: {comport} is not connected");
            }
            else
                Logger.Warn($"Device: {comport} is not connected");
        }

        #endregion
        
        #region Private Methods+
        
        private void DeviceOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = sender as SerialPort;
            var data = new byte[serialPort.BytesToRead];
            serialPort.Read(data, 0, data.Length);
            var eventArgs = new SerialDataReceivedArgs(data, serialPort.PortName);
            DataReceived?.Invoke(this, eventArgs);
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
                Disconnect(entry.Key);
                entry.Value.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        #endregion
        
    }
}