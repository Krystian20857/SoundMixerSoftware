using System;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Extensibility.Loader;
using SoundMixerSoftware.Models;
using Squirrel;

namespace SoundMixerSoftware.ViewModels
{
    public class PluginViewModel : ITabModel
    {
        #region Private Fields
        
        private IWindowManager _windowManager = new WindowManager();
        
        #endregion
        
        #region Public Properties
        
        public static PluginViewModel Instance => IoC.Get<PluginViewModel>();

        public BindableCollection<PluginModel> Plugins { get; set; } = new BindableCollection<PluginModel>();

        public PluginLoader PluginLoader => Bootstrapper.Instance.PluginLoader;
        
        #endregion
        
        #region Implemented Properties

        public string Name { get; set; } = "Plugins";
        public PackIconKind Icon { get; set; } = PackIconKind.Plugin;
        public Guid Uuid { get; set; } = new Guid("F9AE93F6-4E66-479B-AF53-74727BCC9056");

        #endregion
        
        #region Constructor

        public PluginViewModel()
        {
            PluginLoader.PluginLoaded += PluginLoaderOnPluginLoaded;
            foreach (var plugin in PluginLoader.LoadedPlugins)
                PluginLoaderOnPluginLoaded(PluginLoader, new PluginLoadedArgs(plugin.Key, plugin.Value));
        }

        #endregion
        
        #region Public Methods
        
        
        
        #endregion
        
        #region Private Events
        
        private void PluginLoaderOnPluginLoaded(object sender, PluginLoadedArgs e)
        {
            var plugin = e.PluginStruct.Plugin;
            Plugins.Add(PluginModel.CreateModel(plugin));
        }

        public void OpenPluginFolder(object sender)
        {
            var pluginModel = sender as PluginModel;
            var pluginStruct = PluginLoader.LoadedPlugins[pluginModel.PluginId];
            AppUtil.OpenExplorer(pluginStruct.FolderPath);
        }

        public void LoadZipClick()
        {
            _windowManager.ShowDialogAsync(PluginLoadViewModel.Instance);
        }

        public void OpenFolderClick()
        {
            AppUtil.OpenExplorer(PluginLoader.PluginPath);
        }

        public void ReloadAppClick()
        {
            UpdateManager.RestartApp();
        }

        public void OpenPluginSettings(object sender)
        {
            var pluginModel = sender as PluginModel;
            _windowManager.ShowDialogAsync(pluginModel.SettingsWindow);
        }
        
        #endregion
    }
}