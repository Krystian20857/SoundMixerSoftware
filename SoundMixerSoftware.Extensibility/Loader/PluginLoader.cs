using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using NLog;
using NLog.Fluent;
using SoundMixerSoftware.Extensibility.Storage;
using SoundMixerSoftware.Extensibility.Storage.Yaml;

namespace SoundMixerSoftware.Extensibility.Loader
{
    public class PluginLoader
    {
        #region Logger

        /// <summary>
        /// Current class logger.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields
        
        private readonly DirectoryManager _directoryManager;
        private readonly IPluginInfoManager _infoManager;

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Collection of loaded plugins.
        /// </summary>
        public Dictionary<string, PluginStruct> LoadedPlugins { get; } = new Dictionary<string, PluginStruct>();
        /// <summary>
        /// Path to plugins.
        /// </summary>
        public string PluginPath => _directoryManager.PluginPath;
        /// <summary>
        /// Path ro plugins cache.
        /// </summary>
        public string CacheLocation { get; }

        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when plugin has successfully loaded.
        /// </summary>
        public event EventHandler<PluginLoadedArgs> PluginLoaded;
        
        #endregion
        
        #region Constructor

        public PluginLoader(string pluginsPath, string cacheLocation)
        {
            _directoryManager = new DirectoryManager(pluginsPath);
            _infoManager = new YamlPluginInfoManager(_directoryManager.PluginPath);
            CacheLocation = cacheLocation;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                foreach (var pluginPath in _directoryManager.GetPluginFolders(false))
                {
                    var fileName = new AssemblyName(args.Name).Name + ".dll";
                    var assemblyPath = Path.Combine(pluginPath, fileName);
                    if(File.Exists(assemblyPath))
                        return Assembly.LoadFrom(assemblyPath);
                }

                return null;
            };
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Load all detected plugins and catch exceptions.
        /// </summary>
        public void LoadAllPlugins()
        {
            try
            {
                foreach (var pluginDir in _directoryManager.GetPluginFolders())
                    LoadPluginGroup(Path.GetDirectoryName(pluginDir));
            }
            catch (PluginLoadException exception)
            {
                Logger.Error(exception);
            }
        }

        public void ViewLoadingEvent()
        {
            foreach (var plugin in LoadedPlugins)
                plugin.Value.Plugin.OnViewLoading();
        }

        public void ViewLoadedEvent()
        {
            foreach (var plugin in LoadedPlugins)
                plugin.Value.Plugin.OnViewLoaded();
        }

        /// <summary>
        /// Copy content of zip file to cache.
        /// </summary>
        /// <param name="zipFile">Zip file location.</param>
        /// <returns>Path to zip file content.</returns>
        public string CopyZipCache(string zipFile)
        {
            var cacheDir = CreateCacheDir();
            ZipFile.ExtractToDirectory(zipFile, cacheDir);
            return cacheDir;
        }

        /// <summary>
        /// Validate plugin zip file before loading.
        /// </summary>
        /// <param name="zipFile">zip file location</param>
        /// <param name="zipContent">out zip content cache location</param>
        /// <param name="cacheLocation">out cache location of unziped plugin</param>
        /// <param name="finalPluginFolder">out path where plugin can bi installed</param>
        /// <returns></returns>
        /// <exception cref="PluginLoadException"></exception>
        public bool ValidateZipFile(string zipFile, out string zipContent, out string cacheLocation, out string finalPluginFolder)
        {
            zipContent = CopyZipCache(zipFile);
            cacheLocation = Directory.GetDirectories(zipContent).FirstOrDefault();
            if (string.IsNullOrEmpty(cacheLocation))
            {
                Directory.Delete(zipContent, true);
                throw new PluginLoadException($"Cannot extract plugin from: {zipFile}.");
            }

            var pluginInfo = _infoManager.GetInfo(cacheLocation);
            if (pluginInfo == default)
            {
                Directory.Delete(zipContent, true);
                throw new PluginLoadException("Cannot locate info file.");
            }

            if (LoadedPlugins.Keys.Any(x => pluginInfo.Plugins.ContainsKey(x)))
            {
                Directory.Delete(zipContent, true);
                throw new PluginLoadException("Plugin with this id is already loaded.");
            }

            finalPluginFolder = Path.Combine(PluginPath, Path.GetFileName(cacheLocation));
            if (Directory.Exists(finalPluginFolder))
            {
                Directory.Delete(zipContent, true);
                throw new PluginLoadException("Cannot replace installed plugin. To update plugin first remove it from plugins folder.");
            }

            return true;
        }

        /// <summary>
        /// Load plugin from preloaded cached zip.
        /// </summary>
        /// <param name="cacheLocation">location in cache</param>
        /// <param name="pluginLocation">plugin installation path</param>
        public void LoadPreloadedZip(string cacheLocation, string pluginLocation)
        {
            Directory.Move(cacheLocation, pluginLocation);
            LoadPluginGroup(pluginLocation);
        }

        /// <summary>
        /// Clear cache.
        /// </summary>
        /// <param name="cacheFolder">folder og plugin cache</param>
        public bool ClearCache(string cacheFolder)
        {
            if (!Directory.Exists(cacheFolder))
                return false;
            Directory.Delete(Path.Combine(CacheLocation, cacheFolder), true);
            return true;
        }

        public bool LoadFromZip(string zipFile)
        {
            if (!ValidateZipFile(zipFile, out var zipContent, out var cacheLocation, out var finalPluginFolder))
                return false;
            LoadPreloadedZip(cacheLocation, finalPluginFolder);
            ClearCache(Path.GetFileName(zipContent));
            return true;
        }
        
        public void RegisterPlugin(IPlugin instance, Assembly assembly, string id, string pluginPath)
        {
            var plugin = new PluginStruct(instance, assembly, pluginPath);
            
            //Assert for id, version and author
            Debug.Assert(!string.IsNullOrWhiteSpace(instance.PluginId), "Plugin ID cannot be null, empty or white spaced");
            Debug.Assert(!string.IsNullOrWhiteSpace(instance.Version), "Plugin version cannot be null, empty or white spaced");
            Debug.Assert(!string.IsNullOrWhiteSpace(instance.Author), "Plugin author cannot be null, empty or white spaced");

            LoadedPlugins.Add(id, plugin);
            instance.OnPluginLoaded();
            PluginLoaded?.Invoke(this, new PluginLoadedArgs(id, plugin));
            Logger.Debug($"Loaded {id} plugin.");
        }
        
        #endregion
        
        #region Private Methods

        /// <summary>
        /// Load plugins from specified folder.
        /// </summary>
        /// <param name="folderPath"></param>
        private void LoadPluginGroup(string folderPath)
        {
            var pluginInfo = _infoManager.GetInfo(folderPath);
            if (pluginInfo == default)
            {
                throw new PluginLoadException($"Cannot read plugin info file: {folderPath}.");
                //Logger.Warn($"Cannot read plugin info file: {folderPath}.");
                //return;
            }
            Logger.Debug($"Loading {folderPath} plugin group.");

            if (!_directoryManager.GetDirectory(folderPath, out var assemblyPath))
            {
                throw new PluginLoadException($"Cannot find assembly path of plugin: {folderPath}.");
                //Logger.Warn($"Cannot find assembly path of plugin: {folderPath}.");
                //return;
            }
            
            var ignoreList = new List<string>();
            foreach (var plugin in pluginInfo.Plugins)
                ignoreList.Add(plugin.Value.ModuleName);
            if (pluginInfo.LoadOrder.Count == 0)
            {
                var dependencies = AssemblyLoader.LoadAssemblies(assemblyPath, ignoreList);
                if(dependencies.Any())
                    Logger.Debug($"Loaded plugin dependencies: {folderPath} -> {FormatAssemblies(dependencies)}");
            }
            else
            {
                foreach (var assembly in pluginInfo.LoadOrder)
                {
                    var assemblyFile = Path.Combine(assemblyPath, assembly);
                    if (!File.Exists(assemblyFile))
                    {
                        Logger.Warn($"Cannot find assembly from load order: {assembly}");
                        continue;
                    }

                    Assembly.LoadFrom(assemblyFile);
                }
            }
            //var pluginAssemblies = AssemblyLoader.LoadAssemblyFiles(assemblyPath, ignoreList);
            //Logger.Debug($"Loaded plugin assemblies: {folderPath} -> {FormatAssemblies(pluginAssemblies)}");
            foreach (var plugin in pluginInfo.Plugins)
            {
                var pluginStorage = plugin.Value;
                var pluginAssemblyPath = Path.Combine(assemblyPath, pluginStorage.ModuleName);
                if (!File.Exists(pluginAssemblyPath))
                {
                    throw new PluginLoadException($"Cannot find plugin assembly: {pluginAssemblyPath}");
                }
                var assembly = Assembly.LoadFrom(pluginAssemblyPath);
                Logger.Debug($"Loaded {pluginStorage.ModuleName} assembly");
                LoadSinglePlugin(assembly, plugin.Key, pluginStorage.Class, assemblyPath);
            }
        }

        /// <summary>
        /// Load single plugin from available assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="id"></param>
        /// <param name="entryPoint"></param>
        private void LoadSinglePlugin(Assembly assembly, string id, string entryPoint, string pluginPath)
        {
            var type = assembly.GetType(entryPoint, true, true);
            if (!type.IsPublic || type.IsAbstract)
                throw new PluginLoadException("Plugin class must be public and non-abstract.");
            if (type.IsSubclassOf(typeof(AbstractPlugin)))
            {
                if (LoadedPlugins.ContainsKey(id))
                {
                    Logger.Warn($"Plugin with id: {id} is alredy loaded.");
                }
                else
                {
                    var instance = (AbstractPlugin) Activator.CreateInstance(type, new object[] {this, pluginPath});
                    if (instance.PluginId.Equals(id))
                        RegisterPlugin(instance, assembly, id, pluginPath);
                    else
                        Logger.Warn($"Id: {id} does not match real plugin id: {instance.PluginId}");
                }
            }
            else if (type.GetInterfaces().Contains(typeof(IPlugin)))
            {
                if (LoadedPlugins.ContainsKey(id))
                {
                    Logger.Warn($"Plugin with id: {id} is alredy loaded.");
                }
                else
                {
                    if (type.GetConstructors().Any(x => x.GetParameters().Length != 0))
                    {
                        Logger.Error($"Constructor of plugin with id: {id} must be blank.");
                    }
                    else
                    {
                        var instance = (IPlugin) Activator.CreateInstance(type);
                        if (instance.PluginId.Equals(id))
                            RegisterPlugin(instance, assembly, id, pluginPath);
                        else
                            Logger.Warn($"Id: {id} does not match real plugin id: {instance.PluginId}");
                    }
                }
            }
        }

        private string FormatAssemblies(IEnumerable<Assembly> assemblies)
        {
            return string.Join(",", assemblies.Select(x => x.GetName().Name));
        }

        private string CreateCacheDir()
        {
            var path = Path.Combine(CacheLocation, Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        #endregion

    }

    public class PluginLoadedArgs : EventArgs
    {
        public string PluginId { get; set; }
        public PluginStruct PluginStruct { get; set; }

        public PluginLoadedArgs(string pluginId, PluginStruct pluginStruct)
        {
            PluginId = pluginId;
            PluginStruct = pluginStruct;
        }
    }
}