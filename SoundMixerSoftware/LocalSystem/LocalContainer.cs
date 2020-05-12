using System;
using System.IO;
using System.Reflection;
using SoundMixerSoftware.Common.LocalSystem;

namespace SoundMixerSoftware.LocalSystem
{
    [LocalContainer]
    public static class LocalContainer
    {
        /// <summary>
        /// executable path.
        /// key: "basedir"
        /// </summary>
        [Path("basedir", false, FileAttributes.Directory)]
        public static readonly string ExecPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        /// <summary>
        /// Application Data folder path.
        /// key: "appdata"
        /// </summary>
        [Path("appdata", true, FileAttributes.Directory)]
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName));
        /// <summary>
        /// Configuration file path.
        /// key: "config"
        /// </summary>
        [Path("config", false, FileAttributes.Normal)]
        public static readonly string Config = Path.Combine(AppData, "config.yml");
        /// <summary>
        /// Profiles folder path.
        /// key "profiles"
        /// </summary>
        [Path("profiles", true, FileAttributes.Directory)]
        public static readonly string Profiles = Path.Combine(AppData, "profiles");
        /// <summary>
        /// Logs folder path.
        /// key: "logsdir"
        /// </summary>
        [Path("logsdir", true, FileAttributes.Directory)]
        public static readonly string LogsFolder = Path.Combine(AppData, "logs");
        /// <summary>
        /// latest.log file path.
        /// key: "latestlog"
        /// </summary>
        [Path("latestlog", false, FileAttributes.Normal)]
        public static readonly string LatestLog = Path.Combine(LogsFolder, "latest.log");
        /// <summary>
        /// Cache folder.
        /// key: "latestlog"
        /// </summary>
        [Path("cache", true, FileAttributes.Directory)]
        public static readonly string Cache = Path.Combine(AppData, "cache");
        /// <summary>
        /// Icon cache folder.
        /// key: "iconcache"
        /// </summary>
        [Path("iconcache", true, FileAttributes.Directory)]
        public static readonly string IconCache = Path.Combine(Cache, "iconcache");
    }
}