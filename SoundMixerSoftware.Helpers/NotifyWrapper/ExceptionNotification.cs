using System;
using System.Windows.Forms;
using Notifications.Wpf;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.LocalSystem;

namespace SoundMixerSoftware.Helpers.NotifyWrapper
{
    public class ExceptionNotification : INotification<Exception>
    {
        #region Constants
        
        public const string EXCEPTION_KEY = "EXCEPTION";
        
        #endregion
        
        #region Fields

        private NotificationManager _manager = new NotificationManager();
        private Exception _exception;
        
        #endregion
        
        #region Impelemtened Events
        
        public event Action Clicked;
        public event Action Closed;

        #endregion
        
        #region Constructor

        public ExceptionNotification()
        {
            Clicked += () => AppUtils.OpenExplorer(LocalContainer.LogsFolder);
        }
        
        #endregion
        
        #region Impelemted Methods

        public Exception GetValue(string key)
        {
            return key.Equals(EXCEPTION_KEY) ? _exception : null;
        }

        public void SetValue(string key, Exception value)
        {
            if (key.Equals(EXCEPTION_KEY))
                _exception = value;
        }

        public bool RemoveValue(string key)
        {
            throw new NotImplementedException();
        }

        public Exception[] GetValues()
        {
            throw new NotImplementedException();
        }

        public string[] GetKeys()
        {
            throw new NotImplementedException();
        }
        
        public void Show()
        {
            var content = new NotificationContent
            {
                Type = NotificationType.Error,
                Title = $"Error: {_exception.Message}",
                Message = "For more info check logs or click this notification."
            };
            
            _manager.Show(content, onClose: Closed, onClick: Clicked, expirationTime: TimeSpan.FromMilliseconds(ConfigHandler.ConfigStruct.NotificationShowTime));
        }
        
        #endregion
    }
}