﻿using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Wpf;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.LocalSystem;

namespace SoundMixerSoftware.Framework.NotifyWrapper
{
    public class ExceptionNotification : INotification<string>
    {
        #region Constants
        
        public const string MESSAGE_KEY = "MESSAGE";
        
        #endregion
        
        #region Fields

        private NotificationManager _manager = new NotificationManager();
        private string _message;
        
        #endregion
        
        #region Impelemtened Events
        
        public event Action Clicked;
        public event Action Closed;

        #endregion
        
        #region Constructor

        public ExceptionNotification()
        {
            Clicked += () => AppUtil.OpenExplorer(LocalContainer.LogsFolder);
        }
        
        #endregion
        
        #region Impelemted Methods

        public string GetValue(string key)
        {
            return key.Equals(MESSAGE_KEY) ? _message : null;
        }

        public void SetValue(string key, string value)
        {
            if (key.Equals(MESSAGE_KEY))
                _message = value;
        }

        public bool RemoveValue(string key)
        {
            //throw new NotImplementedException();
            return true;
        }

        public IEnumerable<string> GetValues()
        {
            //throw new NotImplementedException();
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetKeys()
        {
            return Enumerable.Empty<string>();
        }

        
        public void Show()
        {
            var content = new NotificationContent
            {
                Type = NotificationType.Error,
                Title = $"Error: {_message}",
                Message = "For more info check logs or click this notification."
            };
            
            _manager.Show(content, onClose: Closed, onClick: Clicked, expirationTime: ConfigHandler.ConfigStruct.Notification.NotificationShowTime);
        }
        
        #endregion
    }
}