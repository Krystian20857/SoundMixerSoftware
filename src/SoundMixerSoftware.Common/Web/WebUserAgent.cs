using System;
using System.Reflection;

namespace SoundMixerSoftware.Common.Web
{
    public class WebUserAgent
    {
        public static string Default => $"Mozilla/5.0 (compatible; {Environment.OSVersion.Platform} {Environment.OSVersion.VersionString}; SoundMixerSoftware/{Assembly.GetExecutingAssembly().GetName().Version};)";
    }
}