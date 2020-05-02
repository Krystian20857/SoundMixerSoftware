using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using NLog;
using YamlDotNet.Serialization;

namespace SoundMixerAppv2.Common.Config.Yaml
{
    public class YamlProfileManager<T> : IProfileManager<T>
    {
        #region Logger
        
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Properties
        
        public Dictionary<Guid, T> Profiles { get; set; } = new Dictionary<Guid, T>();
        
        #endregion
        
        #region Private Fields

        private readonly string _profileFolder;
        private readonly SerializationHelper _serializationHelper = new SerializationHelper();
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;
        
        #endregion
        
        #region Constructor

        public YamlProfileManager(string profileFolder)
        {
            _profileFolder = profileFolder;

            _serializer = _serializationHelper.Serializer;
            _deserializer = _serializationHelper.Deserializer;
            
            LoadAll();
        }
        
        #endregion
        
        #region Public Methods
        
        public void Load(Guid uuid)
        {
            if(Profiles.ContainsKey(uuid))
                Logger.Warn($"Profile alredy loaded: {uuid}");
            else
            {
                Profiles.Add(uuid, ReadProfile(uuid));
                Logger.Info($"Loaded profile{uuid}");
            }
        }

        public Guid Create(T profile)
        {
            var uuid = Guid.NewGuid();
            while (Profiles.ContainsKey(uuid))
                uuid = Guid.NewGuid();
            WriteProfile(uuid, profile);
            Logger.Info($"Created profile: {uuid}");
            return uuid;
        }

        public bool Save(Guid uuid)
        {
            if (!Profiles.ContainsKey(uuid))
                return false;
            WriteProfile(uuid, Profiles[uuid]);
            Logger.Info($"Saved profile: {uuid}");
            return true;
        }

        public void LoadAll()
        {
            foreach (var profile in GetProfiles())
                Load(profile);
        }

        public void SaveAll()
        {
            foreach (var profile in GetProfiles())
                Save(profile);
        }

        public IEnumerable<Guid> GetProfiles()
        {
            foreach (var profileFile in Directory.GetFiles(_profileFolder))
            {
                var profileName = Path.GetFileNameWithoutExtension(profileFile);
                if (Guid.TryParse(profileName, out var uuid))
                    yield return uuid;
                else
                    Logger.Warn($"Failed to parse profile name: {profileName}");
            }
        }
        
        #endregion
        
        #region Private Methods

        /// <summary>
        /// Creates profile file path.
        /// </summary>
        /// <param name="uuid">uuid of profile.</param>
        /// <returns>combined path.</returns>
        private string MakeProfileFile(Guid uuid)
        {
            return Path.Combine(_profileFolder, $"{uuid}.yml");
        }

        /// <summary>
        /// Writes profile to specified file.
        /// </summary>
        /// <param name="uuid">uuid of profile</param>
        /// <param name="profile">profile struct</param>
        private void WriteProfile(Guid uuid, T profile)
        {
            File.WriteAllText(MakeProfileFile(uuid), _serializer.Serialize(profile));
        }

        /// <summary>
        /// Read profile from file.
        /// </summary>
        /// <param name="uuid">uuid of profile</param>
        /// <returns>profile struct</returns>
        private T ReadProfile(Guid uuid)
        {
            return _deserializer.Deserialize<T>(File.ReadAllText(MakeProfileFile(uuid), Encoding.UTF8));
        }

        #endregion
    }
}