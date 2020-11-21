using System;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Device;
using Squirrel;

namespace SoundMixerSoftware.Framework.NotifyWrapper
{
    public static class NotificationHandler
    {
        #region Fields
        
        private static INotification<string> _exceptionNotification = new ExceptionNotification();
        private static INotification<ReleaseEntry> _releaseNotification = new NewVersionNotification();
        private static INotification<object> _deviceNotitication = new DeviceNotification();
        
        #endregion
        
        #region Properties
        
        private static bool CanShow => ConfigHandler.ConfigStruct.Notification.EnableNotifications;
        
        #endregion
        
        #region Methods

        public static void ShowErrorNotification(string message, Action onClick = null, Action onClose = null)
        {
            _exceptionNotification.SetValue(ExceptionNotification.MESSAGE_KEY, message);
            ShowNotification(_exceptionNotification, onClick, onClose);
        }

        public static void ShowNewVersionNotification(ReleaseEntry release, Action onClick = null, Action onClose = null)
        {
            _releaseNotification.SetValue(NewVersionNotification.VERSION_KEY, release);
            ShowNotification(_releaseNotification, onClick, onClose);
        }

        public static void ShowDeviceConnectedNotification(DevicePair devicePair, Action onClick = null, Action onClose = null)
        {
            _deviceNotitication.SetValue(DeviceNotification.DEVICE_STATE_KEY, DeviceNotificationState.Connected);
            _deviceNotitication.SetValue(DeviceNotification.EVENT_ARGS_KEY, devicePair);
            ShowNotification(_deviceNotitication, onClick, onClose);
        }
        
        public static void ShowDeviceDisconnectedNotification(DevicePair devicePair, Action onClick = null, Action onClose = null)
        {
            _deviceNotitication.SetValue(DeviceNotification.DEVICE_STATE_KEY, DeviceNotificationState.Disconnected);
            _deviceNotitication.SetValue(DeviceNotification.EVENT_ARGS_KEY, devicePair);
            ShowNotification(_deviceNotitication, onClick, onClose);
        }

        private static void ShowNotification<T>(INotification<T> notification, Action onClick, Action onClose)
        {
            if(!CanShow) return;
            
            notification.Clicked += onClick ?? (() => {});
            notification.Closed += onClose ?? (() => {});
            notification.Show();
        }
        
        #endregion
    }
}