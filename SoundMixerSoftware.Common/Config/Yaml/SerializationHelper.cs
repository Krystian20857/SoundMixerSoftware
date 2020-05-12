using NLog;
using YamlDotNet.Serialization;

namespace SoundMixerAppv2.Common.Config.Yaml
{
    public class SerializationHelper
    {
        #region Logger

        /// <summary>
        /// Use for logging in current class.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// YAML serializer.
        /// </summary>
        public ISerializer Serializer { get; private set; }
        /// <summary>
        /// YAML deserializer.
        /// </summary>
        public IDeserializer Deserializer { get; private set; }

        #endregion

        #region Private Fields
        
        /// <summary>
        /// Serializer builder use for creating base Serializer.
        /// </summary>
        private readonly SerializerBuilder _serializerBuilder = new SerializerBuilder();
        /// <summary>
        /// Deserializer builder use for creating base Deserializer
        /// </summary>
        private readonly DeserializerBuilder _deserializerBuilder = new DeserializerBuilder();
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Creates instance of <see cref="SerializationHelper"/>. After instance creating builds <see cref="Serializer"/> and <see cref="Deserializer"/> using <see cref="Build"/> Method
        /// </summary>
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