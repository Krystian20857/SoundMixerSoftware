using System;
using System.Windows.Media;
using SoundMixerSoftware.Extensibility.Loader;

namespace SoundMixerSoftware.Extensibility
{
    public abstract class AbstractPlugin : IPlugin
    {
        #region Public Properties

        public string PluginPath { get; }
        public PluginLoader PluginLoader { get; }

        #endregion
        
        #region Abstract Properties
        
        public abstract string Name { get; set; }
        public abstract ImageSource Image { get; set; }
        public abstract string Author { get; set; }
        public abstract string Version { get; set; }
        public abstract string PluginId { get; set; }
        public abstract Uri WebPage { get; set; }

        #endregion

        #region Constructor

        protected AbstractPlugin(PluginLoader pluginLoader, string pluginPath)
        {
            PluginPath = pluginPath;
            PluginLoader = pluginLoader;
        }
        
        #endregion
        
        #region Virtual Methods
        
        public virtual void OnPluginLoaded()
        {
            
        }

        public virtual void OnViewLoading()
        {
            
        }

        public virtual void OnViewLoaded()
        {
            
        }
        
        #endregion
    }
}