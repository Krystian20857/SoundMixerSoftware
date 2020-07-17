using System.Reflection;

namespace SoundMixerSoftware.Extensibility
{
    public class PluginStruct
    {
        /// <summary>
        /// Gets plugin object.
        /// </summary>
        public IPlugin Plugin { get; }
        /// <summary>
        /// Gets plugins assembly.
        /// </summary>
        public Assembly Assembly { get; }
        /// <summary>
        /// Gets plugin folder path.
        /// </summary>
        public string FolderPath { get;}

        public PluginStruct(IPlugin plugin, Assembly assembly, string folderPath)
        {
            Plugin = plugin;
            Assembly = assembly;
            FolderPath = folderPath;
        }
    }
}