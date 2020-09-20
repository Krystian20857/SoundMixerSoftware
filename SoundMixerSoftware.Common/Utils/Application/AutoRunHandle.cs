using System.IO;
using Microsoft.Win32;

namespace SoundMixerSoftware.Common.Utils.Application
{
    /// <summary>
    /// Helps with autorun handling.
    /// </summary>
    public class AutoRunHandle
    {
        #region Const

        public const string RUN_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        
        #endregion
        
        #region Private Fields

        private string _appPath;
        private string _appName;

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets application path.
        /// </summary>
        public string AppPath
        {
            get => _appPath;
            set
            {
                _appPath = value;
                _appName = Path.GetFileNameWithoutExtension(value);
            }
        }

        /// <summary>
        /// Get application name;
        /// </summary>
        public string AppName => _appName;

        #endregion
        
        #region Private Properties
        
        /// <summary>
        /// Run registry key object.
        /// </summary>
        private RegistryKey RunKey => Registry.CurrentUser.OpenSubKey(RUN_PATH, true);
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Create startup handle instance.
        /// </summary>
        /// <param name="appPath"></param>
        public AutoRunHandle(string appPath)
        {
            AppPath = appPath;
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Check if application is in startup.
        /// </summary>
        /// <returns></returns>
        public bool CheckInstance(bool setCurrentPath = true)
        {
            var keyExists = RunKey?.GetValue(AppName) != null;
            if(setCurrentPath && keyExists)
                RunKey.SetValue(AppName, AppPath);
            return keyExists;
        }
        
        /// <summary>
        /// Set application to startup.
        /// </summary>
        public void SetStartup()
        {
            if (CheckInstance())
                return;
            RunKey.SetValue(AppName, AppPath);
        }

        /// <summary>
        /// Remove application from startup.
        /// </summary>
        public void RemoveStartup()
        {
            if (!CheckInstance())
                return;
            RunKey.DeleteValue(AppName, false);
        }
        
        #endregion
    }
}