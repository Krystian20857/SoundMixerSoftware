using System;
using NLog;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Framework.LocalSystem;
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
            NotificationHandler.ShowErrorNotification(message);
        }
        
    }
}