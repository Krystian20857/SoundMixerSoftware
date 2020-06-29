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
        #region CONST

        public static readonly Guid VolumeUUID = new Guid("DBD9BCEE-06FB-44E5-9842-53C9A309782A");
        
        #endregion

        #region Private Fields

        /// <summary>
        /// Helps with audio devices enumeration.
        /// </summary>
        private MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        private Dictionary<string, VolumeHandler> _volumeHandlers = new Dictionary<string, VolumeHandler>();
        
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
        public MMDevice DefaultInput => _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

        public string DefaultInputID;
        public string DefaultOutputID;

        /// <summary>
        /// Gets all devices.
        /// </summary>
        public IEnumerable<MMDevice> AllDevices => OutputDevices.Concat(InputDevices);
        
        #endregion

        #region Events

        /// <summary>
        /// Fires when device state changed.
        /// </summary>
        public event EventHandler<DeviceStateChangedArgs> DeviceStateChanged;
        /// <summary>
        /// Fires when new audio device has connected.
        /// </summary>
        public event EventHandler DeviceAdded;
        /// <summary>
        /// Fires when audio device has removed.
        /// </summary>
        public event EventHandler DeviceRemoved;
        /// <summary>
        /// Fires when Default device has changed.
        /// </summary>
        public event EventHandler<DefaultDeviceChangedArgs> DefaultDeviceChange;
        /// <summary>
        /// Fires when device property has changed.
        /// </summary>
        public event EventHandler<PropertyChangedArgs> PropertyValueChanged;
        /// <summary>
        /// Fires when device volume has changed.
        /// </summary>
        public event EventHandler<VolumeChangedArgs> DeviceVolumeChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Create device enumerator instance with registered events.
        /// </summary>
        public DeviceEnumerator()
        {
            _deviceEnumerator.RegisterEndpointNotificationCallback(this);
            foreach (var device in AllDevices)
                RegisterEvents(device);
            DefaultInputID = DefaultInput.ID;
            DefaultOutputID = DefaultOutput.ID;
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

        /// <summary>
        /// Get MMDevice by string id.
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns>MMDevice</returns>
        public MMDevice GetDeviceById(string id) => _deviceEnumerator.GetDevice(id);

        #endregion
        
        #region Private Methods

        public void RegisterEvents(MMDevice device)
        {
            if (_volumeHandlers.ContainsKey(device.ID))
                _volumeHandlers.Remove(device.ID);
            var volumeHandler = new VolumeHandler(device);
            device.AudioEndpointVolume.NotificationGuid = VolumeUUID;
            volumeHandler.VolumeChanged += VolumeHandlerOnVolumeChanged;
            _volumeHandlers.Add(device.ID, volumeHandler);
        }

        private void VolumeHandlerOnVolumeChanged(object sender, VolumeChangedArgs e)
        {
            DeviceVolumeChanged?.Invoke(sender, e);
        }

        #endregion

        #region Implemented Methods

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            var device = _deviceEnumerator.GetDevice(deviceId);
            DeviceStateChanged?.Invoke(device, new DeviceStateChangedArgs(newState));
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            var device = _deviceEnumerator.GetDevice(pwstrDeviceId);
            RegisterEvents(device);
            DeviceAdded?.Invoke(device, new EventArgs());
        }

        public void OnDeviceRemoved(string deviceId)
        {
            var device = _deviceEnumerator.GetDevice(deviceId);
            DeviceRemoved?.Invoke(device, new EventArgs());
        }
        
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == DataFlow.Render)
                DefaultOutputID = string.Copy(defaultDeviceId);
            else if (flow == DataFlow.Capture)
                DefaultInputID = string.Copy(defaultDeviceId);
            DefaultDeviceChange?.Invoke(defaultDeviceId, new DefaultDeviceChangedArgs(flow, role));
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            PropertyValueChanged?.Invoke(_deviceEnumerator.GetDevice(pwstrDeviceId), new PropertyChangedArgs(key));
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose..........
        /// </summary>
        public void Dispose()
        {
            _deviceEnumerator.UnregisterEndpointNotificationCallback(this);
            foreach (var handler in _volumeHandlers)
                handler.Value.Dispose();
            _volumeHandlers.Clear();
            _deviceEnumerator?.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
    
    public class VolumeHandler : IDisposable
    {
        public event EventHandler<VolumeChangedArgs> VolumeChanged;

        public MMDevice Device { get; }

        public VolumeHandler(MMDevice device)
        {
            Device = device;
            device.AudioEndpointVolume.NotificationGuid = DeviceEnumerator.VolumeUUID;
            device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolumeOnOnVolumeNotification;
        }

        private void AudioEndpointVolumeOnOnVolumeNotification(AudioVolumeNotificationData data)
        {
            VolumeChanged?.Invoke(Device, new VolumeChangedArgs(data.MasterVolume, data.Muted, data.Guid == DeviceEnumerator.VolumeUUID));
        }
        
        public void Dispose()
        {
            Device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolumeOnOnVolumeNotification;
            //Device?.Dispose();
            //GC.SuppressFinalize(this);
        }
    }
}