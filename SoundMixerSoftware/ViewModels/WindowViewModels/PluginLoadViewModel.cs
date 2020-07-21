using System.IO;
using System.Windows.Media;
using Caliburn.Micro;
using Microsoft.Win32;
using NLog;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Extensibility.Loader;
using SoundMixerSoftware.Helpers.Utils;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware.ViewModels
{
    public class PluginLoadViewModel : Screen
    {
        #region Current Class Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Properties
        
        private DebounceDispatcher _debounceDispatcher = new DebounceDispatcher();
        
        private string _zipPath;
        private Brush _statusColor;

        private string zipCacheContent;
        private string pluginCacheLocation;
        private string pluginPredictLocation;

        private bool status;
        
        #endregion
        
        #region Public Properties

        public PluginLoader PluginLoader => Bootstrapper.Instance.PluginLoader;

        public string ZipPath
        {
            get => _zipPath;
            set
            {
                _zipPath = value;
                _debounceDispatcher.Debounce(50,(state) =>
                {
                    if (!File.Exists(value))
                    {
                        SetStatus(false, "File does not exists.");
                        return;
                    }

                    try
                    {
                        PluginLoader.ValidateZipFile(value, out zipCacheContent, out pluginCacheLocation, out pluginPredictLocation);
                    }
                    catch (PluginLoadException exception)
                    {
                        SetStatus(false, exception.Message);
                        zipCacheContent = string.Empty;
                        pluginCacheLocation = string.Empty;
                        pluginPredictLocation = string.Empty;
                        return;
                    }

                    SetStatus(true, "Plugin ready to load!");
                });
                NotifyOfPropertyChange(nameof(ZipPath));
            }
        }

        public string ValidationStatus { get; set; }

        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                NotifyOfPropertyChange(nameof(StatusColor));
            }
        }

        #endregion
        
        #region Constructor
        
        
        
        #endregion
        
        #region Public Methods

        public static string ShowFileDialog()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "ZIP file (*.zip)|*.zip",
                DefaultExt = ".zip"
            };
            var result = fileDialog.ShowDialog() ?? false;
            if (!result)
                return string.Empty;
            return fileDialog.FileName;
        }
        
        #endregion
        
        #region Private Methods

        private void SetStatus(bool status, string message)
        {
            this.status = status;
            if (status)
                StatusColor = new SolidColorBrush(Colors.LimeGreen);
            else
                StatusColor = new SolidColorBrush(Colors.IndianRed);
            ValidationStatus = message;
            NotifyOfPropertyChange(nameof(ValidationStatus));
            NotifyOfPropertyChange(nameof(StatusColor));
        }
        
        #endregion
        
        #region Private Events

        public void OpenZipClicked()
        {
            var fileName = ShowFileDialog();
            if(string.IsNullOrEmpty(fileName))
                return;
            ZipPath = fileName;
        }

        public void LoadClicked()
        {
            try
            {
                if (string.IsNullOrEmpty(pluginCacheLocation) || string.IsNullOrEmpty(pluginPredictLocation) || !status)
                    return;
                PluginLoader.LoadPreloadedZip(pluginCacheLocation, pluginPredictLocation);
                if (!string.IsNullOrEmpty(zipCacheContent))
                    PluginLoader.ClearCache(zipCacheContent);
                Bootstrapper.Instance.ReloadAssembliesForView();
            }
            catch (PluginLoadException ex)
            {
                ExceptionHandler.HandleException(Logger, "Something went wrong while loading plugin.", ex);
            }
            TryCloseAsync();
        }

        public void CancelClicked()
        {
           if(!string.IsNullOrEmpty(zipCacheContent))
               PluginLoader.ClearCache(zipCacheContent);
           TryCloseAsync();
        }
        
        public void CloseClicked()
        {
            if(!string.IsNullOrEmpty(zipCacheContent))
                PluginLoader.ClearCache(zipCacheContent);
        }
        
        #endregion
    }
}