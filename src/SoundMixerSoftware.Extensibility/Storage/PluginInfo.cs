using System.Collections.Generic;

namespace SoundMixerSoftware.Extensibility.Storage
{
    public class PluginInfo
    {
        /// <summary>
        /// Plugins in plugin group.
        /// </summary>
        public Dictionary<string, PluginStorage> Plugins { get; set; } = new Dictionary<string, PluginStorage>();
        /// <summary>
        /// Order of file loading.
        /// </summary>
        public List<string> LoadOrder { get; set; } = new List<string>();
    }

    public class PluginStorage
    {
        /// <summary>
        /// Main class of plugin.
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// Filename of plugin module.
        /// </summary>
        public string ModuleName { get; set; }
    }
}