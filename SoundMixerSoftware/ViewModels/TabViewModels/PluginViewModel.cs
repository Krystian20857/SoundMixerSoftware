using System;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils.Application;
using SoundMixerSoftware.Extensibility;
using SoundMixerSoftware.Extensibility.Loader;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class PluginViewModel : ITabModel
    {
        #region Private Fields
        
        private IWindowManager _windowManager = new WindowManager();
        
        #endregion
        
        #region Public Properties

        public BindableCollection<PluginModel> Plugins { get; set; } = new BindableCollection<PluginModel>();

        public PluginLoader PluginLoader => Bootstrapper.PluginLoader;
        
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
            AppUtils.OpenExplorer(pluginStruct.FolderPath);
        }

        public void LoadZipClick()
        {
            _windowManager.ShowDialogAsync(IoC.Get<PluginLoadViewModel>());
        }

        public void OpenFolderClick()
        {
            AppUtils.OpenExplorer(PluginLoader.PluginPath);
        }

        public void ReloadAppClick()
        {
            StarterHelper.RestartApp();
        }
        
        #endregion
    }
}