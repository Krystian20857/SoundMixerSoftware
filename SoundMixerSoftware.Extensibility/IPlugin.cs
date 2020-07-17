using System;
using System.Windows.Media;

namespace SoundMixerSoftware.Extensibility
{
    public interface IPlugin
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Image of the plugin.
        /// </summary>
        ImageSource Image { get; set; }
        /// <summary>
        /// Author of the plugin.
        /// </summary>
        string Author { get; set; }
        /// <summary>
        /// Version of the plugin.
        /// </summary>
        string Version { get; set; }
        /// <summary>
        ///Identifier of plugin.
        /// </summary>
        string PluginId { get; set; }
        /// <summary>
        /// Occurs when plugin has loaded.
        /// </summary>
        void OnPluginLoaded();
        /// <summary>
        /// Occurs when view is loading.
        /// </summary>
        void OnViewLoading();
        /// <summary>
        /// Occurs after view loaded.
        /// </summary>
        void OnViewLoaded();
    }
}