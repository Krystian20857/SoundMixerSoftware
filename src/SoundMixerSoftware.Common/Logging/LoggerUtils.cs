using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SoundMixerSoftware.Common.Logging
{
    /// <summary>
    /// Helps with logger configuration.
    /// </summary>
    public static class LoggerUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Setting up logger.
        /// </summary>
        /// <param name="logsFolder">Folder path where logs will be stored</param>
        public static void SetupLogger(string logsFolder)
        {
            var config = new LoggingConfiguration();
            var logFile = new FileTarget("logfile")                                                 //logs will be saved at %AppData%/{AppName}/logs/
            {                                                                                             //logs are archived event day to %AppData%/{AppName}/logs/
                FileName = Path.Combine(logsFolder, "latest.log"),                                        //max archived logs files = 30
                KeepFileOpen = false,
                ArchiveFileName = Path.Combine(logsFolder, "log_${shortdate}.{##}.log"),
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 30,
            };

            var consoleLog = new ConsoleTarget()
            {
                AutoFlush = false,
                DetectConsoleAvailable = true
            };

            var eventLog = new EventLogTarget()
            {
                Source = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location),
            };

            var coloredConsole = new ColoredConsoleTarget()
            {
                DetectConsoleAvailable = true,
                DetectOutputRedirected = true,
                AutoFlush = false,
            };
            
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logFile);                   //all logs will be displayed to console and saved to file
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleLog);
            config.AddRule(LogLevel.Fatal, LogLevel.Fatal, eventLog);                  //only fatals and errors will be appear in eventlog
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, coloredConsole);

            LogManager.Configuration = config;
        }

        #endregion
    }
}