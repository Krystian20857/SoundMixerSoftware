using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NLog;
using SoundMixerSoftware.Interop.Constant;
using SoundMixerSoftware.Interop.Method;
using SoundMixerSoftware.Interop.Struct;

namespace SoundMixerSoftware.Interop.USBLib
{
    /// <summary>
    /// Handles USB events.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class USBDevice
    {
        #region Logger

        /// <summary>
        /// Current Class Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        /// <summary>
        /// Device class guid.
        /// </summary>
        private readonly Guid _deviceGuid = Guid.Empty;
        /// <summary>
        /// Connected devices list.
        /// </summary>
        private List<DeviceProperties> _connectedDevices = new List<DeviceProperties>();

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets connected devices.
        /// </summary>
        public IReadOnlyList<DeviceProperties> ConnectedDevices => _connectedDevices.AsReadOnly();

        public List<USBID> VidPid { get; set; } = new List<USBID>();

        #endregion
        
        #region Events

        /// <summary>
        /// Fires when device get connected.
        /// </summary>
        public event EventHandler<DeviceStateArgs> DeviceArrive;
        /// <summary>
        /// Fires when device get disconnected.
        /// </summary>
        public event EventHandler<DeviceStateArgs> DeviceRemove;
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Create instance with specified device type interface guid.
        /// </summary>
        /// <param name="deviceGuid">device class</param>
        public USBDevice(Guid deviceGuid)
        {
            _deviceGuid = deviceGuid;
        }

        /// <summary>
        /// Create instance with specified device type interface guid, vendor id and product id.
        /// </summary>
        /// <param name="deviceGuid">device class</param>
        /// <param name="ids"></param>
        public USBDevice(Guid deviceGuid, IEnumerable<USBID> ids)
        {
            _deviceGuid = deviceGuid;
            VidPid.AddRange(ids);
            CheckDevices();
        }

        /// <summary>
        /// Create instance with specified vendor id and product id.
        /// </summary>
        public USBDevice(IEnumerable<USBID> ids)
        {
            VidPid.AddRange(ids);
            CheckDevices();
        }
        
        /// <summary>
        /// Create instance for all devices.
        /// </summary>
        public USBDevice(){}
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Register specified window to capture WM_DEVICECHANGE message.
        /// </summary>
        /// <param name="handle">Window handle.</param>
        /// <returns></returns>
        public bool RegisterDeviceChange(IntPtr handle)
        {
            var deviceinterface = new DEV_BROADCAST_DEVICEINTERFACE();
            var flags = DEVICE_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE | DEVICE_FLAGS.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES;
            deviceinterface.dbcc_size = Marshal.SizeOf(deviceinterface);
            if (!_deviceGuid.Equals(Guid.Empty))
            {
                deviceinterface.dbcc_classguid = _deviceGuid;
                flags = 0;
            }
            deviceinterface.dbcc_devicetype = (int) DBTDEVTYPE.DBT_DEVTYP_DEVICEINTERFACE;
            var buffer = Marshal.AllocHGlobal(deviceinterface.dbcc_size);
            Marshal.StructureToPtr(deviceinterface, buffer, true);
            var deviceHandle = User32.RegisterDeviceNotification(handle, buffer, flags);
            var status = (deviceHandle != IntPtr.Zero);
            if(!status)
                Logger.Warn($"Win32 Error: {Marshal.GetLastWin32Error()}");
            Marshal.FreeHGlobal(buffer);
            return status;
        }
        
        /// <summary>
        /// Checks connected devices.
        /// </summary>
        /// <returns></returns>
        public bool CheckDevices()
        {
            _connectedDevices = USBDescriptor.GetDescriptors(VidPid).ToList();
            return VidPid.Count > 0;
        }

        /// <summary>
        /// Unregister windows form receiving WM_DEVICECHANGE message.
        /// </summary>
        /// <param name="handle"></param>
        public bool UnregisterDeviceChange(IntPtr handle)
        {
            return User32.UnregisterDeviceNotification(handle);
        }

        /// <summary>
        /// Process windows message for device change.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        public void ProcessMessage(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message != WM.WM_DEVICECHANGE) return;

            var devices = new DeviceProperties[_connectedDevices.Count];
            _connectedDevices.CopyTo(devices);
            switch (wParam.ToInt32())
            {
                case DBTEVENT.DBT_DEVICEARRIVAL:
                    var deviceTypeArrive = (uint)Marshal.ReadInt32(lParam, 4);
                    var connectedDevice = default(DeviceProperties);
                    _connectedDevices = USBDescriptor.GetDescriptors(VidPid).ToList();
                        connectedDevice = _connectedDevices.Except(devices).FirstOrDefault();
                    DeviceArrive?.Invoke(this, new DeviceStateArgs(deviceTypeArrive, connectedDevice));
                    break;
                case DBTEVENT.DBT_DEVICEREMOVECOMPLETE:
                    var deviceTypeRemove = (uint)Marshal.ReadInt32(lParam, 4);
                    var disconnectedDevice = default(DeviceProperties);
                    _connectedDevices = USBDescriptor.GetDescriptors(VidPid).ToList();
                        disconnectedDevice = _connectedDevices.Any()
                                ? devices.Except(_connectedDevices).FirstOrDefault()
                                : devices.FirstOrDefault();
                    DeviceRemove?.Invoke(this, new DeviceStateArgs(deviceTypeRemove, disconnectedDevice));
                    break;
            }
        }

        #endregion
    }
}