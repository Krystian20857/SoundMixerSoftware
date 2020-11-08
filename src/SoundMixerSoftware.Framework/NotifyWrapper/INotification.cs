using System;
using SoundMixerSoftware.Common.Property;

namespace SoundMixerSoftware.Framework.NotifyWrapper
{
    public interface INotification<T> : IProperties<string, T>
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