using System;
using System.Diagnostics;
using System.IO;
using NLog;

namespace SoundMixerSoftware.Updater.Installer
{
    public class InstallerStarter
    {
        #region Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Properties
        
        public string FileName { get; set; }
        public Action<InstallerStarter> AppExit { get; set; }

        #endregion

        #region Public Methods

        public void RunInstaller(UpdateMode mode)
        {
            var args = " ";
            switch (mode)
            {
                case UpdateMode.SILENT:
                    args += "/VERYSILENT /FORCECLOSEAPPLICATIONS";
                    break;
                case UpdateMode.NORMAL:
                    break;
            }

            if (!File.Exists(FileName))
            {
                Logger.Error("Cannot find installer file.");
                return;
            }

            Process.Start(FileName, args);
            //AppExit.Invoke(this);
        }
        
        #endregion
    }
}