using System;
using Notifications.Wpf;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Device;

namespace SoundMixerSoftware.Helpers.NotifyWrapper
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
            throw new NotImplementedException();
        }

        public object[] GetValues()
        {
            throw new NotImplementedException();
        }

        public string[] GetKeys()
        {
            throw new NotImplementedException();
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
            _notificationManager.Show(content, onClick: Clicked, onClose: Closed, expirationTime: TimeSpan.FromMilliseconds(ConfigHandler.ConfigStruct.NotificationShowTime));
        }
        
        #endregion
    }
}