using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NLog;

namespace SoundMixerSoftware.Common.AudioLib
{
    public class DeviceEnumerator : IDisposable, IMMNotificationClient
    {
        #region Logger

        /// <summary>
        /// Current Class Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Private Fields

        /// <summary>
        /// Helps with audio devices enumeration.
        /// </summary>
        private MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        
        private Dictionary<Guid, MMDevice> _devices = new Dictionary<Guid, MMDevice>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets active output devices.
        /// </summary>
        public MMDeviceCollection OutputDevices =>
            _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        /// <summary>
        /// Gets active input devices.
        /// </summary>
        public MMDeviceCollection InputDevices =>
            _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

        /// <summary>
        /// Gets default multimedia output device.
        /// </summary>
        public MMDevice DefaultOutput => _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        /// <summary>
        /// Gets default multimedia input device.
        /// </summary>
        public MMDevice DefaultInput => _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        #endregion

        #region Events

        /// <summary>
        /// Fires when device state changed.
        /// </summary>
        public event Action<MMDevice, DeviceState> DeviceStateChanged;
        /// <summary>
        /// Fires when new audio device has connected.
        /// </summary>
        public event Action<MMDevice> DeviceAdded;
        /// <summary>
        /// Fires when audio device has removed.
        /// </summary>
        public event Action<MMDevice> DeviceRemoved;
        /// <summary>
        /// Fires when Default device has changed.
        /// </summary>
        public event Action<MMDevice, DataFlow, Role> DefaultDeviceChange;
        /// <summary>
        /// Fires when device property has changed.
        /// </summary>
        public event Action<MMDevice, PropertyKey> PropertyValueChanged;
        /// <summary>
        /// Fires when device volume has changed.
        /// </summary>
        public event Action<MMDevice, float, bool> DeviceVolumeChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Create device enumerator instance with registered events.
        /// </summary>
        public DeviceEnumerator()
        {
            _deviceEnumerator.RegisterEndpointNotificationCallback(this);
            foreach (var device in OutputDevices.Concat(InputDevices))
                RegisterEvents(device);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets default device with specified role and data flow.
        /// </summary>
        /// <param name="flow">data flow</param>
        /// <param name="role">role</param>
        /// <returns></returns>
        public MMDevice GetDefaultEndpoint(DataFlow flow, Role role) =>
            _deviceEnumerator.GetDefaultAudioEndpoint(flow, role);

        #endregion
        
        #region Private Methods

        private void RegisterEvents(MMDevice device)
        {
            var uuid = Guid.NewGuid();
            while(_devices.ContainsKey(uuid))
                uuid = Guid.NewGuid();
            device.AudioEndpointVolume.NotificationGuid = uuid;
            device.AudioEndpointVolume.OnVolumeNotification += VolumeNotification; 
        }

        private void RemoveDevice(Guid uuid)
        {
            if (!_devices.ContainsKey(uuid)) return;
            var device = _devices[uuid];
            device.AudioEndpointVolume.OnVolumeNotification -= VolumeNotification;
            _devices.Remove(uuid);
        }

        private void VolumeNotification(AudioVolumeNotificationData data)
        {
            var device = _devices[data.Guid];
            DeviceVolumeChanged?.Invoke(device, data.MasterVolume, data.Muted);
        }

        #endregion

        #region Implemented Methods

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            DeviceStateChanged?.Invoke(_deviceEnumerator.GetDevice(deviceId), newState);
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            var device = _deviceEnumerator.GetDevice(pwstrDeviceId);
            RegisterEvents(device);
            DeviceAdded?.Invoke(device);
        }

        public void OnDeviceRemoved(string deviceId)
        {
            DeviceRemoved?.Invoke(_deviceEnumerator.GetDevice(deviceId));
        }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            DefaultDeviceChange?.Invoke(_deviceEnumerator.GetDevice(defaultDeviceId), flow, role);
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            PropertyValueChanged?.Invoke(_deviceEnumerator.GetDevice(pwstrDeviceId), key);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose..........
        /// </summary>
        public void Dispose()
        {
            _deviceEnumerator?.Dispose();
        }

        #endregion
    }
}