using System;
using System.Collections.Generic;
using Notifications.Wpf;
using SoundMixerSoftware.Helpers.Device;

namespace SoundMixerSoftware.Helpers.NotifyWrapper
{
    public class DeviceNotification : INotification
    {
        #region Private Fields
        
        private NotificationManager _notificationManager = new NotificationManager();

        #endregion
        
        #region Implemented Events
        
        public event Action Clicked;
        public event Action Closed;
        
        #endregion
        
        #region Public Proeprties

        public DeviceConnectedEventArgs Device { get; set; }
        public DeviceNotificationState State { get; set; }

        #endregion
        
        #region Implemented Methods
        
        public DeviceConnectedEventArgs GetValue(string key)
        {
            throw new NotImplementedException();
        }

        public bool RemoveValue(string key)
        {
            throw new NotImplementedException();
        }

        public DeviceConnectedEventArgs[] GetValues()
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
            if (State == DeviceNotificationState.Connected)
            {
                content = new NotificationContent
                {
                    Title = $"Device Connected: {Device.DeviceResponse.name}",
                    Message = $"ComPort: {Device.Device.COMPort}\n",
                    Type = NotificationType.Information
                };
            }
            else if (State == DeviceNotificationState.Disconnected)
            {
                content = new NotificationContent
                {
                    Title = $"Device Disconnected: {Device.DeviceResponse.name}",
                    Message = $"ComPort: {Device.Device.COMPort}\n",
                    Type = NotificationType.Error
                };
            }
            _notificationManager.Show(content, onClick: Clicked, onClose: Closed, expirationTime: TimeSpan.FromSeconds(7));
        }
        
        #endregion
    }
}