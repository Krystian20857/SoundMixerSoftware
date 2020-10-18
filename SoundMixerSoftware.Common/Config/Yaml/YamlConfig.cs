using System;
using System.IO;
using System.Text;
using NLog;
using SoundMixerSoftware.Common.Utils;
using YamlDotNet.Serialization;

namespace SoundMixerSoftware.Common.Config.Yaml
{
    public class YamlConfig<T> : IConfig<T> where T : IConfigStruct<T>
    {
        #region Logger

        /// <summary>
        /// Use for logging in current class.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Properties

        public T Config
        {
            get => _config;
            set => _config = value;
        }

        #endregion
        
        #region Private Fields

        private T _config = Activator.CreateInstance<T>();
        /// <summary>
        /// Absolute config file path.
        /// </summary>
        private readonly string _configPath;
        /// <summary>
        /// SerializationHelper use for creating Serializer and Deserializer for current instance.
        /// </summary>
        private readonly SerializationHelper _serializationHelper = new SerializationHelper();
        /// <summary>
        /// YAML Serializer.
        /// </summary>
        private readonly ISerializer _serializer;
        /// <summary>
        /// YAML Deserializer.
        /// </summary>
        private readonly IDeserializer _deserializer;
        /// <summary>
        /// Store SampleConfig from Config.
        /// </summary>
        private T _sampleConfig;

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

            _serializer = _serializationHelper.Serializer;
            _deserializer = _serializationHelper.Deserializer;
                
            LoadConfig();
        }
        
        #endregion
        
        #region Implemented Methods
        
        public void LoadConfig()
        {
            if (File.Exists(_configPath))
            {
                Config = _deserializer.Deserialize<T>(ReadYAML(_configPath));
                if (ObjectUtil.MergeObjects(ref _config, _sampleConfig))
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
            WriteYAML(_configPath, _serializer.Serialize(Config));
            Logger.Info($"Saved config: {_configPath}");
        }

        public void SaveSampleConfig()
        {
            Config = (T)_sampleConfig.Clone();
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