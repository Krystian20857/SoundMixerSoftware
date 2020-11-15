using System;
using System.Collections.Generic;
using Notifications.Wpf;
using SoundMixerSoftware.Framework.Config;
using Squirrel;

namespace SoundMixerSoftware.Framework.NotifyWrapper
{
    public class NewVersionNotification : INotification<ReleaseEntry>
    {
        #region Const

        public const string VERSION_KEY = "VERSION";
        
        #endregion
        
        #region Private Fields
        
        private INotificationManager _manager = new NotificationManager();
        
        #endregion
        
        #region Public Properties

        public ReleaseEntry NewVersion { get; set; }

        #endregion
        
        #region Implemented Events
        
        public event Action Clicked;
        public event Action Closed;
        
        #endregion
        
        #region Implemented Methods
        
        public ReleaseEntry GetValue(string key)
        {
            return NewVersion;
        }

        public void SetValue(string key, ReleaseEntry value)
        {
            switch (key)
            {
                case VERSION_KEY:
                    NewVersion = value;
                    break;
            }
        }

        public bool RemoveValue(string key)
        {
            switch (key)
            {
                case VERSION_KEY:
                    NewVersion = default;
                    return true;
                default: return false;
            }
        }

        public IEnumerable<ReleaseEntry> GetValues()
        {
            return new[] { NewVersion };
        }

        public IEnumerable<string> GetKeys()
        {
            return new[] { VERSION_KEY };
        }
        public void Show()
        {
            var content = new NotificationContent();
            content.Type = NotificationType.Information;
            content.Title = "Sound Mixer Software Update";
            if (NewVersion == default)
            {
                content.Message = "You are running newest version.";
            }
            else
            {
                content.Message = "New version has downloaded. Click to update.";
            }
            _manager.Show(content, onClick: Clicked, onClose: Closed, expirationTime: ConfigHandler.ConfigStruct.Notification.NotificationShowTime);
        }

        #endregion

    }
}