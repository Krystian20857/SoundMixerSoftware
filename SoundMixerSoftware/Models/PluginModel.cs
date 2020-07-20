using System;
using System.Windows.Media;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Extensibility;
using SoundMixerSoftware.Helpers.Utils;

namespace SoundMixerSoftware.Models
{
    public class PluginModel
    {
        #region Public Properties
        
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string PluginId { get; set; }
        public Uri WebPage { get; set; }
        public object SettingsWindow { get; set; }
        public bool SettingsVisible { get; set; }

        #endregion
        
        #region Public Static Methods

        public static PluginModel CreateModel(IPlugin plugin)
        {
            return new PluginModel
            {
                Name = plugin.Name,
                Image = plugin.Image ?? Resource.RestartImage.ToImageSource(),
                Author = plugin.Author,
                Version = plugin.Version,
                PluginId = plugin.PluginId,
                WebPage = plugin.WebPage,
                SettingsWindow = plugin.SettingsWindow,
                SettingsVisible = plugin.SettingsWindow != null
            };
        }
        
        #endregion
    }
}