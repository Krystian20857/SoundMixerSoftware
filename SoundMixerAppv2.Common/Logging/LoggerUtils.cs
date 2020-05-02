using System.IO;
using System.Reflection;
using NLog;
using NLog.Targets;
using SoundMixerAppv2;

namespace SoundMixerAppv2.Common.Logging
{
    /// <summary>
    /// Helps with logger configuration.
    /// </summary>
    public static class LoggerUtils
    {

        #region Public Static Methods
        /// <summary>
        /// Setup logger.
        /// </summary>
        public static void SetupLogger(string logsFolder)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logFile = new NLog.Targets.FileTarget("logfile")                                    //logs will be saved at %AppData%/{AppName}/logs/
            {                                                                                             //logs are archived event day to %AppData%/{AppName}/logs/
                FileName = Path.Combine(logsFolder, "latest.log"),                                        //max archived logs files = 30
                KeepFileOpen = false,
                ArchiveFileName = Path.Combine(logsFolder, "log_${shortdate}.{##}.log"),
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 30,
            };

            var consoleLog = new NLog.Targets.ConsoleTarget()
            {
                AutoFlush = false,
            };

            var eventLog = new NLog.Targets.EventLogTarget()
            {
                Source = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location),
            };
            
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logFile);                   //all logs will be displayed to console and saved to file
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleLog);
            config.AddRule(LogLevel.Error, LogLevel.Fatal, eventLog);                  //only fatals and errors will be appear in eventlog

            NLog.LogManager.Configuration = config;
            
        }

        #endregion
    }
}