using System.Diagnostics;
using System.Reflection;
using SoundMixerSoftware.Framework.LocalSystem;

namespace SoundMixerSoftware
{
    public static class Constant
    {
        
#if DEBUG
        public const string GITHUB_REPO_URL = "https://github.com/Krystian20857/SoundMixerSoftware";
#else
        public const string GITHUB_REPO_URL = "https://github.com/Krystian20857/SoundMixerSoftware";
#endif
        
        public static readonly string BuildConfiguration = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>().Configuration;
        public static readonly string AppVersion = FileVersionInfo.GetVersionInfo(LocalContainer.ExecPath).ProductVersion;
    }
}