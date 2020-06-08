﻿using System;
using Notifications.Wpf;
using SoundMixerSoftware.Common.Property;
using SoundMixerSoftware.Helpers.Device;

namespace SoundMixerSoftware.Helpers.NotifyWrapper
{
    public interface INotification : IProperties<string, DeviceConnectedEventArgs>
    {
        /// <summary>
        /// Attach onClick event.
        /// </summary>
        event Action Clicked;
        /// <summary>
        /// Attach onClose event.
        /// </summary>
        event Action Closed;
        /// <summary>
        /// Show notification.
        /// </summary>
        void Show();
    }
}