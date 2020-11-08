using System.Collections.Generic;
using System.IO;
using NLog;
using YamlDotNet.Serialization;

namespace SoundMixerSoftware.Extensibility.Storage.Yaml
{
    public class YamlPluginInfoManager : IPluginInfoManager
    {
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private readonly DirectoryManager _directoryManager;
        private readonly IDeserializer _deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();
        private readonly ISerializer _serializer = new SerializerBuilder().IgnoreFields().Build();
        
        #endregion
        
        #region Implemented Properties
        
        public Dictionary<string, PluginInfo> Storage { get; } = new Dictionary<string, PluginInfo>();

        #endregion
        
        #region Constructor

        public YamlPluginInfoManager(string pluginsFolder)
        {
            _directoryManager = new DirectoryManager(pluginsFolder);
        }
        
        #endregion
        
        #region Implemented Methods
        
        public PluginInfo GetInfo(string folderName)
        {
            if (!_directoryManager.GetDefinitionFileFromFolder(folderName, out var infoFile))
            {
                Logger.Warn($"Unable to locate plugin info file: {infoFile}");
                SetStorage(folderName, default);
                return default;
            }

            var pluginInfo = LoadYaml(infoFile);
            SetStorage(folderName, pluginInfo);
            return pluginInfo;
        }

        public void SetInfo(string folderName, PluginInfo pluginInfo)
        {
            if (!_directoryManager.GetDefinitionFileFromFolder(folderName, out var infoFile))
            {
                Logger.Info($"Unable to locate plugin info file: {infoFile}. Creating new one.");
            }
            SetStorage(folderName, pluginInfo);
            SaveYaml(pluginInfo, infoFile);
        }

        public void LoadAll()
        {
            foreach (var pluginFolder in _directoryManager.GetPluginFolders(false))
            {
                GetInfo(Path.GetFileName(pluginFolder));
            }
        }

        #endregion
        
        #region Private Methods

        private PluginInfo LoadYaml(string file)
        {
            return _deserializer.Deserialize<PluginInfo>(File.ReadAllText(file));
        }

        private void SaveYaml(PluginInfo pluginInfo, string file)
        {
            File.WriteAllText(file, _serializer.Serialize(pluginInfo));
        }

        private void SetStorage(string key, PluginInfo value)
        {
            if (Storage.ContainsKey(key))
                Storage[key] = value;
            else
                Storage.Add(key, value);
        }

        #endregion
    }
}