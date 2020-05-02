using System;
using System.Linq;
using NLog;
using SoundMixerAppv2.Common.Config;
using SoundMixerAppv2.Common.Config.Yaml;
using SoundMixerAppv2.Common.LocalSystem;
using SoundMixerAppv2.Common.Logging;
using SoundMixerAppv2.LocalSystem;

namespace SoundMixerAppv2
{
    public static class Program
    {
        #region Logger
        
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        #endregion
        
        #region Public Properites
        
        public static LocalManager LocalManager { get; } = new LocalManager(typeof(LocalContainer));

        #endregion
        
        #region Main Method

        public static void Main(string[] args)
        {
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            LocalManager.ResolveLocal();
            
            IProfileManager<TestProfile> profileManager = new YamlProfileManager<TestProfile>(LocalContainer.Profiles);
            profileManager.LoadAll();
            foreach (var profile in profileManager.Profiles)
            {
                Console.WriteLine($"{profile.Key}:\n\tAge: {profile.Value.Age}\n\tName: {profile.Value.Name}\n\tSurname: {profile.Value.Surname}");
            }
        }

        private class TestProfile
        {
            public int Age { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
        } 
        
        #endregion
    }
}
