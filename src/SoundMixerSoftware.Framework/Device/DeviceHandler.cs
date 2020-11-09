using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using SoundMixerSoftware.Common.Communication;
using SoundMixerSoftware.Common.Communication.Serial;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Interop.Constant;
using SoundMixerSoftware.Interop.USBLib;
using SoundMixerSoftware.Interop.Wrapper;

namespace SoundMixerSoftware.Framework.Device
{
    public class DeviceHandler : IDisposable
    {
        #region Current Class Logger

        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
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
        /// <summary>
        /// Contains detected devices flag on application startup
        /// </summary>
        private List<byte> _flagsOnStartup = new List<byte>();
        /// <summary>
        /// Contains data converters for each connected device.
        /// </summary>
        private Dictionary<string, DataConverter> _dataConverters = new Dictionary<string, DataConverter>();
        /// <summary>
        /// Use in synchronization between data converters and devicehandler.
        /// </summary>
        private Dictionary<byte, Type> _typesToRegister = new Dictionary<byte, Type>();
        /// <summary>
        /// Indicates if serial data event has subscribed.
        /// </summary>
        private bool _serialEventRegistered = false;
        private readonly object _lockObject = new object();

        #endregion
        
        #region Private Properties

        #endregion
        
        #region Public Properties

        public IntPtr NativeHandle => _windowWrapper.Handle;
        public Dictionary<string, DeviceId> ConnectedDevices { get; set; } = new Dictionary<string, DeviceId>();

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
            
            _usbDevice = new USBDevice(GUID_DEVINTERFACE.GUID_DEVINTERFACE_PARALLEL, ConfigHandler.ConfigStruct.Hardware.UsbIDs);
            _usbDevice.VidPid = ConfigHandler.ConfigStruct.Hardware.UsbIDs;                                                            //Provides configuration synchronization
            _usbDevice.RegisterDeviceChange(_windowWrapper.Handle);
            
            RegisterTypes();
            
            _serialConnection = new SerialConnection(ConfigHandler.ConfigStruct.Hardware.SerialConfig);
            _serialConnection.DeviceConnected += SerialConnectionOnDeviceConnected;
            _serialConnection.ExceptionOccurs += SerialConnectionOnExceptionOccurs;
            
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
        public void RegisterType(byte command, Type type)
        { 
            if(!_typesToRegister.ContainsKey(command))
                _typesToRegister.Add(command, type);
            foreach (var converter in _dataConverters)
                converter.Value.RegisterType(command, type);    
        }

        /// <summary>
        /// Unregister type from data conversion.
        /// </summary>
        /// <param name="command"></param>
        public void UnRegisterType(byte command)
        {
            if(_typesToRegister.ContainsKey(command))
                _typesToRegister.Remove(command);
            foreach (var converter in _dataConverters)
                converter.Value.UnregisterType(command);  
        }

        /// <summary>
        /// SendBytes from serial connection.
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="data"></param>
        public void SendData(string comport, byte[] data)
        {
            _serialConnection.SendBytes(comport, data);
            _serialConnection.SendBytes(comport, new []{ConfigHandler.ConfigStruct.Hardware.Terminator});
        }

        /// <summary>
        /// SendData from serial connection.
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="data"></param>
        public void SendData<T>(string comport, T data) where T : struct
        {
            _serialConnection.SendData(comport, data);
            _serialConnection.SendBytes(comport, new []{ConfigHandler.ConfigStruct.Hardware.Terminator});
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Register internal types.
        /// </summary>
        internal void RegisterTypes()
        {
            RegisterType((byte)Command.DEVICE_RESPONSE_COMMAND, typeof(DeviceIdResponse));
        }
        
        private void SerialConnectionOnExceptionOccurs(object sender, Exception e)
        {
            ExceptionHandler.HandleException(Logger,  "Serial Connection Error" ,e);
        }

        /// <summary>
        /// Create dataconverter for specified serial device.
        /// </summary>
        /// <param name="comPort"></param>
        private void CreateDataConverter(string comPort)
        {
            if (!_dataConverters.ContainsKey(comPort))
            {
                var dataConverter = new DataConverter(ConfigHandler.ConfigStruct.Hardware.Terminator);
                foreach(var type in _typesToRegister)
                    dataConverter.RegisterType(type.Key, type.Value);
                dataConverter.DataReceived += DataConverterOnDataReceived;
                _dataConverters.Add(comPort, dataConverter);
                if (!_serialEventRegistered)
                    _serialConnection.DataReceived += (sender, args) =>
                    {
                        if (_dataConverters.ContainsKey(args.COMPort))
                        {
                            var deviceId = ConnectedDevices.ContainsKey(args.COMPort) ? ConnectedDevices[args.COMPort] : new DeviceId();
                            _dataConverters[args.COMPort].ProcessData(args.Data, deviceId);
                        }
                    };
                _serialEventRegistered = true;
            }
        }

        /// <summary>
        /// Check connected devices. When device has detected DeviceConnected event fires.
        /// </summary>
        private void CheckConnectedDevices()
        {
            foreach (var device in _usbDevice.ConnectedDevices)
            {
                var comPort = device.COMPort;
                CreateDataConverter(comPort);
                _serialConnection.Connect(comPort, false);
                _serialConnection.SendData(comPort, CreateDeviceRequest(device, true));
                _serialConnection.SendBytes(comPort, new byte[] {0xFF});
            }
        }

        /// <summary>
        /// Simple methods for creating device ID requests.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private DeviceIdRequest CreateDeviceRequest(DeviceProperties properties, bool isOnStartup)
        {
            var request = new DeviceIdRequest {command = Command.DEVICE_REQUEST_COMMAND};
            byte flag = 0x01;
            while (_requestFlags.ContainsKey(flag) && _flagsOnStartup.Contains(flag) && flag < 254)
                flag++;
            request.flag = flag;
            _requestFlags.Add(flag, properties);
            if(isOnStartup)
                _flagsOnStartup.Add(flag);
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
            CreateDataConverter(e.COMPort);
            if (!_requestProperties.ContainsKey(e.COMPort))
            {
                DeviceIdRequestError?.Invoke(this, new EventArgs());
                _requestProperties.Clear();
                return;
            }
            var properties = _requestProperties[e.COMPort];
            _serialConnection.SendData(e.COMPort,CreateDeviceRequest(properties, false));
            _serialConnection.SendBytes(e.COMPort, new byte[]{0xFF});
            _requestProperties.Remove(e.COMPort);
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
                lock (_lockObject)
                {
                    DeviceIdResponse response = e.Data;
                    var flag = response.flag;
                    if (_requestFlags.ContainsKey(flag))
                    {
                        var devproperties = _requestFlags[flag];
                        _requestFlags.Remove(response.flag);
                        var startup = _flagsOnStartup.Contains(flag);
                        Logger.Info($"Recived device response: {devproperties.COMPort}");
                        DeviceConnected?.Invoke(this, new DeviceConnectedEventArgs(devproperties, response, startup));
                        ConnectedDevices.Add(devproperties.COMPort, new DeviceId(response.uuid));
                        if (startup)
                            _flagsOnStartup.Remove(flag);
                    }
                    else
                    {
                        _requestFlags.Clear();
                        DeviceIdRequestError?.Invoke(this, new EventArgs());
                    }
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
            lock (_lockObject)
            {
                var comPort = e.DeviceProperties.COMPort;
                if(string.IsNullOrEmpty(comPort))
                    return;
                _serialConnection.Disconnect(comPort);
                DeviceDisconnected?.Invoke(this, e);
                if (_dataConverters.ContainsKey(comPort))
                    _dataConverters.Remove(comPort);
                if (ConnectedDevices.ContainsKey(comPort))
                    ConnectedDevices.Remove(comPort);
            }
        }

        /// <summary>
        /// Handle physical device connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsbDeviceOnDeviceArrive(object sender, DeviceStateArgs e)
        {
            lock (_lockObject)
            {
                if (e.DeviceProperties.Equals(default))
                    return;
                if (string.IsNullOrEmpty(e.DeviceProperties.COMPort))
                    return;
                _requestProperties.Add(e.DeviceProperties.COMPort, e.DeviceProperties);
                _serialConnection.Connect(e.DeviceProperties.COMPort);
            }
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
        
        #region Dispose

        public void Dispose()
        {
            _serialConnection?.Dispose();
        }
        
        #endregion
    }
}