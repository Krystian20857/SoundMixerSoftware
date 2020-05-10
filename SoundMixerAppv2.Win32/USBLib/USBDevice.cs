using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NLog;
using SoundMixerAppv2.Win32.Win32;

namespace SoundMixerAppv2.Win32.USBLib
{
    /// <summary>
    /// Handles USB events.
    /// </summary>
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
        /// <summary>
        /// Gets and Sets vendor ID.
        /// </summary>
        public uint VID { get; set; }
        /// <summary>
        /// Gets and Sets product ID.
        /// </summary>
        public uint PID { get; set; }

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
        /// <param name="vid">vendor id</param>
        /// <param name="pid">product id</param>
        public USBDevice(Guid deviceGuid, uint vid, uint pid)
        {
            _deviceGuid = deviceGuid;
            VID = vid;
            PID = pid;
            CheckDevices();
        }

        /// <summary>
        /// Create instance with specified vendor id and product id.
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="pid"></param>
        public USBDevice(uint vid, uint pid)
        {
            VID = vid;
            PID = pid;
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
            var deviceinterface = new NativeStructs.DEV_BROADCAST_DEVICEINTERFACE();
            var flags = NativeClasses.DEVICE_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE | NativeClasses.DEVICE_FLAGS.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES;
            deviceinterface.dbcc_size = Marshal.SizeOf(deviceinterface);
            if (!_deviceGuid.Equals(Guid.Empty))
            {
                deviceinterface.dbcc_classguid = _deviceGuid;
                flags = 0;
            }
            deviceinterface.dbcc_devicetype = (int) NativeClasses.DBTDEVTYPE.DBT_DEVTYP_DEVICEINTERFACE;
            var buffer = Marshal.AllocHGlobal(deviceinterface.dbcc_size);
            Marshal.StructureToPtr(deviceinterface, buffer, true);
            var deviceHandle = NativeMethods.RegisterDeviceNotification(handle, buffer, flags);
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
            if (VID == 0 && PID == 0)
                return false;
            _connectedDevices = USBDescriptor.GetDescriptors(VID, PID).ToList();
            return true;
        }

        /// <summary>
        /// Unregister windows form receiving WM_DEVICECHANGE message.
        /// </summary>
        /// <param name="handle"></param>
        public bool UnregisterDeviceChange(IntPtr handle)
        {
            return NativeMethods.UnregisterDeviceNotification(handle);
        }

        /// <summary>
        /// Process windows message for device change.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        public void ProcessMessage(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message != NativeClasses.WM.WM_DEVICECHANGE) return;

            var devices = new DeviceProperties[_connectedDevices.Count];
            _connectedDevices.CopyTo(devices);
            switch (wParam.ToInt32())
            {
                case NativeClasses.DBTEVENT.DBT_DEVICEARRIVAL:
                    var deviceTypeArrive = (uint)Marshal.ReadInt32(lParam, 4);
                    var connectedDevice = default(DeviceProperties);
                    if (VID != 0 && PID != 0)
                    {
                        _connectedDevices = USBDescriptor.GetDescriptors(VID, PID).ToList();
                        connectedDevice = _connectedDevices.Except(devices).FirstOrDefault();
                    }
                    DeviceArrive?.Invoke(this, new DeviceStateArgs(deviceTypeArrive, connectedDevice));
                    break;
                case NativeClasses.DBTEVENT.DBT_DEVICEREMOVECOMPLETE:
                    var deviceTypeRemove = (uint)Marshal.ReadInt32(lParam, 4);
                    var disconnectedDevice = default(DeviceProperties);
                    if (VID != 0 && PID != 0)
                    {
                        _connectedDevices = USBDescriptor.GetDescriptors(VID, PID).ToList();
                        disconnectedDevice = _connectedDevices.Any()
                                ? devices.Except(_connectedDevices).First()
                                : devices.FirstOrDefault();
                    }
                    DeviceRemove?.Invoke(this, new DeviceStateArgs(deviceTypeRemove, disconnectedDevice));
                    break;
            }
        }

        #endregion
    }
}