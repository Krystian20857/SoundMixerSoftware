using System;
using NLog;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.NotifyWrapper;

namespace SoundMixerSoftware.Framework.Utils
{
    /// <summary>
    /// Contains useful exception commands.
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Logs exception and notify user.
        /// </summary>
        /// <param name="logger">Logger to log exception</param>
        /// <param name="message">Message to display in user notification.</param>
        /// <param name="exception">Exception to report/log.</param>
        public static void HandleException(Logger logger, string message, Exception exception)
        {
            logger?.Error(exception);
            if (!ConfigHandler.ConfigStruct.Notification.EnableNotifications)
                return;
            var exceptionNotification = new ExceptionNotification();
            exceptionNotification.SetValue(ExceptionNotification.MESSAGE_KEY, message);
            exceptionNotification.Show();
        }
        
    }
}