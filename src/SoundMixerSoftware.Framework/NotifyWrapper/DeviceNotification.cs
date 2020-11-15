using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Wpf;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Device;

namespace SoundMixerSoftware.Framework.NotifyWrapper
{
    public class DeviceNotification : INotification<object>
    {
        #region Constants

        public const string EVENT_ARGS_KEY = "EVENTARGS";
        public const string DEVICE_STATE_KEY = "DEVICESTATE";
        
        #endregion
        
        #region Private Fields
        
        private NotificationManager _notificationManager = new NotificationManager();
        private DeviceConnectedEventArgs device;
        private DeviceNotificationState state;
        #endregion
        
        #region Implemented Events
        
        public event Action Clicked;
        public event Action Closed;
        
        #endregion

        #region Implemented Methods
        
        public object GetValue(string key)
        {
            switch (key)
            {
                case EVENT_ARGS_KEY:
                    return device;
                case DEVICE_STATE_KEY:
                    return state;
                default:
                    return null;
            }
        }

        public void SetValue(string key, object value)
        {
            switch (key)
            {
                case EVENT_ARGS_KEY:
                    device = value as DeviceConnectedEventArgs;
                    break;
                case DEVICE_STATE_KEY:
                    state = (DeviceNotificationState)value;
                    break;
            }
        }

        public bool RemoveValue(string key)
        {
            //throw new NotImplementedException();
            return true;
        }

        public IEnumerable<object> GetValues()
        {
            //throw new NotImplementedException();
            return Enumerable.Empty<object>();
        }

        public IEnumerable<string> GetKeys()
        {
            return Enumerable.Empty<string>();
        }
        
        public void Show()
        {
            var content = new NotificationContent();
            if (state == DeviceNotificationState.Connected)
            {
                content = new NotificationContent
                {
                    Title = $"Device Connected: {device.DeviceResponse.name}",
                    Message = $"ComPort: {device.Device.COMPort}\n",
                    Type = NotificationType.Information
                };
            }
            else if (state == DeviceNotificationState.Disconnected)
            {
                content = new NotificationContent
                {
                    Title = $"Device Disconnected: {device.DeviceResponse.name}",
                    Message = $"ComPort: {device.Device.COMPort}\n",
                    Type = NotificationType.Warning
                };
            }
            _notificationManager.Show(content, onClick: Clicked, onClose: Closed, expirationTime: ConfigHandler.ConfigStruct.Notification.NotificationShowTime);
        }
        
        #endregion
    }
    
    public enum DeviceNotificationState
    {
        Connected,
        Disconnected
    }
}