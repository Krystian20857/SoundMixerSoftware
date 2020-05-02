using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using SoundMixerAppv2.Common.Utils;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace SoundMixerAppv2.Common.Config.Yaml
{
    public class YamlConfig<T> : IConfig<T> where T : IConfigStruct<T>
    {
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Properties

        public T Config { get; set; } = Activator.CreateInstance<T>();

        #endregion
        
        #region Private Fields

        private readonly string _configPath;
        private readonly SerializationHelper _serializationHelper = new SerializationHelper();
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;
        private T _sampleConfig;
        private List<Type> _ignoredTypes = new List<Type>();

        #endregion
        
        #region Constructor

        /// <summary>
        /// Creates IConfig(YamlConfig) instance.
        /// </summary>
        /// <param name="configPath">Path to configuration file.</param>
        public YamlConfig(string configPath)
        {
            _configPath = configPath;
            
            _sampleConfig = Config.GetSampleConfig();
            _ignoredTypes.Add(typeof(T));
                
            _serializer = _serializationHelper.Serializer;
            _deserializer = _serializationHelper.Deserializer;
                
            LoadConfig();
        }
        
        #endregion
        
        #region Public Methods
        
        public void LoadConfig()
        {
            if (File.Exists(_configPath))
            {
                Config = _deserializer.Deserialize<T>(ReadYAML(_configPath));
                if (ObjectUtils.MergeObjects(_sampleConfig, Config, _ignoredTypes))
                {
                    SaveConfig();
                    Logger.Info($"Updated config: {_configPath}");
                }
            }
            else
                SaveSampleConfig();
            Logger.Info($"Loaded config: {_configPath}");
        }

        public void SaveConfig()
        {
            WriteYAML(_configPath, _serializer.Serialize(_sampleConfig));
            Logger.Info($"Saved config: {_configPath}");
        }

        public void SaveSampleConfig()
        {
            Config = _sampleConfig.Copy();
            SaveConfig();
            Logger.Info($"Saved sample config: {_configPath}");
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Writes YAML string to specified file.
        /// </summary>
        /// <param name="file">file.</param>
        /// <param name="content">Yaml content.</param>
        private void WriteYAML(string file, string content)
        {
            File.WriteAllText(file, content, Encoding.UTF8);
        }

        /// <summary>
        /// Read YAML from specified file.
        /// </summary>
        /// <param name="file">file.</param>
        /// <returns>YAML content</returns>
        private string ReadYAML(string file)
        {
            return File.ReadAllText(file, Encoding.UTF8);
        }

        #endregion
    }
}