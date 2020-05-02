using NLog;
using YamlDotNet.Serialization;

namespace SoundMixerAppv2.Common.Config.Yaml
{
    public class SerializationHelper
    {
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Serializer.
        /// </summary>
        public ISerializer Serializer { get; private set; }
        /// <summary>
        /// Deserializer.
        /// </summary>
        public IDeserializer Deserializer { get; private set; }

        #endregion

        #region Private Fields
        
        private readonly SerializerBuilder _serializerBuilder = new SerializerBuilder();
        private readonly DeserializerBuilder _deserializerBuilder = new DeserializerBuilder();
        
        #endregion
        
        #region Constructor

        public SerializationHelper()
        {
            Build();
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Build serializer and deserializer with default settings.
        /// </summary>
        public void Build()
        {
            _serializerBuilder.IgnoreFields();
            Serializer = _serializerBuilder.Build();
            Logger.Info("Serializer Created.");

            _deserializerBuilder.IgnoreFields();
            Deserializer = _deserializerBuilder.Build();
            Logger.Info("Deserializer Created.");
        }
        
        #endregion
    }
}