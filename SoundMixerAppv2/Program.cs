using System;
using NLog;
using SoundMixerAppv2.Common.LocalSystem;
using SoundMixerAppv2.Common.Logging;
using SoundMixerAppv2.LocalSystem;

namespace SoundMixerAppv2
{
    public static class Program
    {
        #region Logger
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        #endregion
        
        #region Public Properites
        
        public static LocalManager LocalManager { get; } = new LocalManager(typeof(LocalContainer));

        #endregion
        
        #region Main Method

        public static void Main(String[] args)
        {
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            LocalManager.ResolveLocal();
        }
        
        #endregion
    }
}
