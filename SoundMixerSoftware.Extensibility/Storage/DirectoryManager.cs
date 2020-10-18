using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SoundMixerSoftware.Extensibility.Storage
{
    public class DirectoryManager
    {
        #region Private Properties
        
        
        #endregion
        
        #region Public Properties

        public string PluginPath { get; }

        #endregion
        
        #region Constrcutor

        public DirectoryManager(string pluginPath)
        {
            PluginPath = pluginPath;
        }
        
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Create predict of definition file from plugin folder name;
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public bool GetDefinitionFile(string folderName, out string file)
        {
            file = string.Empty;
            var pluginPath = Path.Combine(PluginPath, folderName);
            if (!Directory.Exists(pluginPath))
                return false;
            file = Path.Combine(pluginPath,$"{Path.GetFileName(pluginPath)}.yml");
            return File.Exists(file);
        }

        public bool GetDefinitionFileFromFolder(string folderName, out string file)
        {
            file = null;
            if(GetDirectory(folderName, out var directory))
                if (GetDefinitionFile(directory, out file))
                    return true;
            return false;
        }

        /// <summary>
        /// Get plugin folders;
        /// </summary>
        /// <param name="requireDefFile"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPluginFolders(bool requireDefFile = true)
        {
            foreach (var path in Directory.GetDirectories(PluginPath))
            {
                if (requireDefFile)
                {
                    if (GetDefinitionFile(path, out var defFile))
                        yield return defFile;
                }
                else
                    yield return path;
            }
        }

        public bool GetDirectory(string pluginFolder, out string fullPluginDirectory)
        {
            fullPluginDirectory = Path.Combine(PluginPath, pluginFolder);
            return Directory.Exists(fullPluginDirectory);
        }

        #endregion
        
        #region Private Methods

        public static void Test()
        {
            /*
            var infoManager = new YamlPluginInfoManager(@"C:\Users\kryst\AppData\Roaming\SoundMixerSoftware\plugins");
            infoManager.LoadAll();
            foreach (var plugin in infoManager.Storage)
            {
                if(plugin.Value == null)
                    infoManager.SetInfo(plugin.Key, new PluginInfo{UUID = Guid.NewGuid()});
                else
                    Console.WriteLine($"{plugin.Key} : UUID: {plugin.Value.UUID}");
            }
            */
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                
            }
        }

        #endregion
    }
}