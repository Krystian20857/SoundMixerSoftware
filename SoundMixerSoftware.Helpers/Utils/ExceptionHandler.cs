using System;
using NLog;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.NotifyWrapper;

namespace SoundMixerSoftware.Helpers.Utils
{
    /// <summary>
    /// Contains useful exception commands.
    /// </summary>
    public static class ExceptionHandler
    {

        /// <summary>
        /// Logs exception and inform user.
        /// </summary>
        /// <param name="exception"></param>
        public static void HandleException(Logger logger,Exception exception)
        {
            logger.Error(exception);
            if (!ConfigHandler.ConfigStruct.EnableNotifications)
                return;
            var exceptionNotification = new ExceptionNotification();
            exceptionNotification.SetValue(ExceptionNotification.EXCEPTION_KEY, exception);
            exceptionNotification.Show();
        }
        
    }
}