using System;
using System.IO;
using System.Reflection;
using SoundMixerSoftware.Common.LocalSystem;

namespace SoundMixerSoftware.Framework.LocalSystem
{
    [LocalContainer]
    public static class LocalContainer
    {
        [Path("execpath", false, FileAttributes.Directory)]
        public static readonly string ExecPath = Assembly.GetExecutingAssembly().Location;
        
        [Path("basedir", false, FileAttributes.Directory)]
        public static readonly string ExecDirectory = Path.GetDirectoryName(ExecPath);
        
        [Path("appdata", true, FileAttributes.Directory)]
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName));
        
        [Path("config", false, FileAttributes.Normal)]
        public static readonly string Config = Path.Combine(AppData, "config.yml");
        
        [Path("profiles", true, FileAttributes.Directory)]
        public static readonly string Profiles = Path.Combine(AppData, "profiles");
        
        [Path("logsdir", true, FileAttributes.Directory)]
        public static readonly string LogsFolder = Path.Combine(AppData, "logs");
        
        [Path("latestlog", false, FileAttributes.Normal)]
        public static readonly string LatestLog = Path.Combine(LogsFolder, "latest.log");
        
        [Path("cache", true, FileAttributes.Directory)]
        public static readonly string Cache = Path.Combine(AppData, "cache");
        
        [Path("plugin", true, FileAttributes.Directory)]
        public static readonly string PluginFolder = Path.Combine(AppData, "plugins");
        
        [Path("plugincache", true, FileAttributes.Directory)]
        public static readonly string PluginCache = Path.Combine(Cache, "plugins");
        
        [Path("imagespath", true, FileAttributes.Directory)]
        public static readonly string ImagesPath = Path.Combine(ExecDirectory, "Images");
    }
}