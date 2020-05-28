using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NAudio.MediaFoundation;
using SoundMixerSoftware.Common.Communication;
using SoundMixerSoftware.Common.Communication.Serial;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Win32.USBLib;
using SoundMixerSoftware.Win32.Utils;
using SoundMixerSoftware.Win32.Win32;
using DataReceivedEventArgs = SoundMixerSoftware.Common.Communication.DataReceivedEventArgs;

namespace SoundMixerSoftware.Helpers.Device
{
    public class DeviceHandler
    {
        #region Private Fields

        /// <summary>
        /// Usb device instance is responsible for handling WM_DEVICECHANGE message.
        /// </summary>
        private readonly USBDevice _usbDevice;
        /// <summary>
        /// Serial connection instance is responsible for handling serial connection.
        /// </summary>
        private readonly SerialConnection _serialConnection;
        /// <summary>
        /// Data converter instance is responsible for handling data conversion.
        /// </summary>
        private readonly DataConverter _dataConverter;
        
        /// <summary>
        /// Stores flags needed to read Device ID.
        /// </summary>
        private Dictionary<byte, DeviceProperties> _requestFlags = new Dictionary<byte, DeviceProperties>();
        /// <summary>
        /// Stores devices requested for serial connection.
        /// </summary>
        private Dictionary<string, DeviceProperties> _requestProperties = new Dictionary<string, DeviceProperties>();
        /// <summary>
        /// Native Window Wrapper used for receiving messages.
        /// </summary>
        private NativeWindowWrapper _windowWrapper = new NativeWindowWrapper();

        #endregion
        
        #region Private Properties

        #endregion
        
        #region Public Properties

        public IntPtr NativeHandle => _windowWrapper.Handle;

        #endregion
        
        #region Events

        /// <summary>
        /// Fires when device ID has gotten properly.
        /// </summary>
        public event EventHandler<DeviceConnectedEventArgs> DeviceConnected;
        /// <summary>
        /// Fires when device has disconnected.
        /// </summary>
        public event EventHandler<DeviceStateArgs> DeviceDisconnected;
        /// <summary>
        /// Fires when error occurs while getting device ID.
        /// </summary>
        public event EventHandler<EventArgs> DeviceIdRequestError;
        /// <summary>
        /// Fires When error occurs while data transfer.
        /// </summary>
        public event EventHandler<EventArgs> DataReceiveError;
        /// <summary>
        /// Occurs when data has received.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Create instance of DeviceHandler and create most important modules.
        /// </summary>
        public DeviceHandler()
        {
            if (!ConfigHandler.IsInitialize())
                ConfigHandler.Initialize();
            
            _usbDevice = new USBDevice(NativeClasses.GUID_DEVINTERFACE.GUID_DEVINTERFACE_PARALLEL, ConfigHandler.ConfigStruct.UsbIDs);
            _usbDevice.VidPid = ConfigHandler.ConfigStruct.UsbIDs;
            _usbDevice.RegisterDeviceChange(_windowWrapper.Handle);

            _dataConverter = new DataConverter(ConfigHandler.ConfigStruct.Terminator);
            _dataConverter.DataReceived += DataConverterOnDataReceived;
            _dataConverter.SizeError += (sender, args) => DataReceiveError?.Invoke(this, new EventArgs());
            RegisterTypes();
            
            _serialConnection = new SerialConnection(ConfigHandler.ConfigStruct.SerialConfig);
            _serialConnection.DataReceived += (sender, args) => _dataConverter.ProcessData(args.Data);
            _serialConnection.DeviceConnected += SerialConnectionOnDeviceConnected;
            
            _usbDevice.DeviceArrive += UsbDeviceOnDeviceArrive;
            _usbDevice.DeviceRemove += UsbDeviceOnDeviceRemove;
            _windowWrapper.MessageReceived += WindowWrapperOnMessageReceived;
            
            CheckConnectedDevices();
        }

        #region Public Methods

        /// <summary>
        /// Register type for data conversion.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="type"></param>
        public void RegisterType(byte command, Type type) => _dataConverter.RegisterType(command, type);
        /// <summary>
        /// Unregister type from data conversion.
        /// </summary>
        /// <param name="command"></param>
        public void UnRegisterType(byte command) => _dataConverter.UnregisterType(command);

        /// <summary>
        /// SendBytes from serial connection.
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="data"></param>
        public void SendData(string comport, byte[] data)
        {
            _serialConnection.SendBytes(comport, data);
            _serialConnection.SendBytes(comport, new []{ConfigHandler.ConfigStruct.Terminator});
        }

        /// <summary>
        /// SendData from serial connection.
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="data"></param>
        public void SendData<T>(string comport, T data) where T : struct
        {
            _serialConnection.SendData(comport, data);
            _serialConnection.SendBytes(comport, new []{ConfigHandler.ConfigStruct.Terminator});
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Register internal types.
        /// </summary>
        internal void RegisterTypes()
        {
            _dataConverter.RegisterType(0x03, typeof(DeviceIdResponse));
        }

        /// <summary>
        /// Check connected devices. When device has detected DeviceConnected event fires.
        /// </summary>
        private void CheckConnectedDevices()
        {
            foreach (var device in _usbDevice.ConnectedDevices)
            {
                var comPort = device.COMPort;
                _serialConnection.Connect(comPort);
                _serialConnection.SendData(comPort,CreateDeviceRequest(device));
                _serialConnection.SendBytes(comPort, new byte[]{0xFF});
            }
        }

        /// <summary>
        /// Simple methods for creating device ID requests.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private DeviceIdRequest CreateDeviceRequest(DeviceProperties properties)
        {
            var request = new DeviceIdRequest {command = 0x02};
            byte flag = 0x01;
            while (_requestFlags.ContainsKey(flag) && flag < 254)
                flag++;
            request.flag = flag;
            _requestFlags.Add(flag, properties);
            return request;
        }
        
        #endregion

        #endregion
        
        #region SerialConnection Events

        /// <summary>
        /// Handle device serial connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialConnectionOnDeviceConnected(object sender, DeviceStateChangeArgs e)
        {
            if (!_requestProperties.ContainsKey(e.COMPort))
            {
                DeviceIdRequestError?.Invoke(this, new EventArgs());
                _requestProperties.Clear();
                return;
            }
            var properties = _requestProperties[e.COMPort];
            _serialConnection.SendData(e.COMPort,CreateDeviceRequest(properties));
            _requestProperties.Remove(e.COMPort);
            _serialConnection.SendBytes(e.COMPort, new byte[]{0xFF});
        }
        
        #endregion
        
        #region DataProcessor Events

        /// <summary>
        /// Handle Received Data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataConverterOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Command == 0x03)
            {
                DeviceIdResponse response = e.Data;
                if (_requestFlags.ContainsKey(response.flag))
                {
                    var devproperties = _requestFlags[response.flag];
                    _requestFlags.Remove(response.flag);
                    DeviceConnected?.Invoke(this, new DeviceConnectedEventArgs(devproperties, response));
                }
                else
                {
                    _requestFlags.Clear();
                    DeviceIdRequestError?.Invoke(this, new EventArgs());
                }
            }
            DataReceived?.Invoke(sender, e);
        }
        
        #endregion
        
        #region UsbDevice Events

        /// <summary>
        /// Handle physical device connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsbDeviceOnDeviceRemove(object sender, DeviceStateArgs e)
        {
            _serialConnection.Disconnect(e.DeviceProperties.COMPort);
            DeviceDisconnected?.Invoke(this, e);
        }

        /// <summary>
        /// Handle physical device connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsbDeviceOnDeviceArrive(object sender, DeviceStateArgs e)
        {
            _requestProperties.Add(e.DeviceProperties.COMPort, e.DeviceProperties);
            _serialConnection.Connect(e.DeviceProperties.COMPort);
        }
        
        #endregion
        
        #region MessageLoop

        /// <summary>
        /// Main message loop.
        /// </summary>
        /// <param name="m"></param>
        private void WindowWrapperOnMessageReceived(object sender, Message e)
        {
            _usbDevice.ProcessMessage((uint)e.Msg, e.WParam, e.LParam);
        }

        #endregion
    }
}