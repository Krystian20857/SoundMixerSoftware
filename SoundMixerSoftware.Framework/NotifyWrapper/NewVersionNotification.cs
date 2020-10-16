using System;
using System.Collections.Generic;
using Notifications.Wpf;
using SoundMixerSoftware.Updater;

namespace SoundMixerSoftware.Helpers.NotifyWrapper
{
    public class NewVersionNotification : INotification<NewVersionEventArgs>
    {
        #region Const

        public const string VERSION_KEY = "VERSION";
        
        #endregion
        
        #region Private Fields
        
        private INotificationManager _manager = new NotificationManager();
        
        #endregion
        
        #region Public Properties

        public NewVersionEventArgs NewVersion { get; set; }

        #endregion
        
        #region Implemented Events
        
        public event Action Clicked;
        public event Action Closed;
        
        #endregion
        
        #region Implemented Methods
        
        public NewVersionEventArgs GetValue(string key)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, NewVersionEventArgs value)
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

        public IEnumerable<NewVersionEventArgs> GetValues()
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
                content.Message = "New version has been released click to update.";
            }
            _manager.Show(content, onClick: Clicked, onClose: Closed);
        }

        #endregion

    }
}