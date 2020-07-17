using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
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

        #endregion
        
        #region Constructor

        public PluginLoader(string pluginsPath)
        {
            _directoryManager = new DirectoryManager(pluginsPath);
            _infoManager = new YamlPluginInfoManager(_directoryManager.PluginPath);
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Load all detected plugins.
        /// </summary>
        public void LoadAllPlugins()
        {
            foreach (var pluginDir in _directoryManager.GetPluginFolders())
                LoadPluginGroup(Path.GetDirectoryName(pluginDir));
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
                Logger.Warn($"Cannot read plugin info file: {folderPath}.");
                return;
            }
            Logger.Debug($"Loading {folderPath} plugin group.");

            if (!_directoryManager.GetDirectory(folderPath, out var assemblyPath))
            {
                Logger.Warn($"Cannot find assembly path of plugin: {folderPath}.");
            }
            
            var ignoreList = new List<string>();
            foreach (var plugin in pluginInfo.Plugins)
                ignoreList.Add(plugin.Value.ModuleName);
            if (pluginInfo.LoadOrder.Count == 0)
            {
                var dependencies = AssemblyLoader.LoadAssemblies(assemblyPath, ignoreList);
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
            var pluginAssemblies = AssemblyLoader.LoadAssemblyFiles(assemblyPath, ignoreList);
            Logger.Debug($"Loaded plugin assemblies: {folderPath} -> {FormatAssemblies(pluginAssemblies)}");
            foreach (var plugin in pluginInfo.Plugins)
            {
                var pluginStorage = plugin.Value;
                LoadSinglePlugin(pluginAssemblies, plugin.Key, pluginStorage.Class, assemblyPath);
            }
        }

        /// <summary>
        /// Load single plugin from available assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="id"></param>
        /// <param name="entryPoint"></param>
        private void LoadSinglePlugin(IEnumerable<Assembly> assemblies, string id, string entryPoint, string pluginPath)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.FullName?.Equals(entryPoint, StringComparison.InvariantCultureIgnoreCase) ?? false)
                        continue;
                    if(!type.IsPublic || type.IsAbstract)
                        continue;
                    if (type.IsSubclassOf(typeof(AbstractPlugin)))
                    {
                        if (LoadedPlugins.ContainsKey(id))
                        {
                            Logger.Warn($"Plugin with id: {id} is alredy loaded.");
                        }
                        else
                        {
                            var instance = (AbstractPlugin) Activator.CreateInstance(type, new object[] {this, pluginPath});
                            LoadedPlugins.Add(id, new PluginStruct(instance, assembly, pluginPath));
                            instance.OnPluginLoaded();
                            Logger.Debug($"Loaded {id} plugin.");
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
                                LoadedPlugins.Add(id, new PluginStruct(instance, assembly, pluginPath));
                                instance.OnPluginLoaded();
                                Logger.Debug($"Loaded {id} plugin.");
                            }
                        }
                    }
                }
            }
        }



        private string FormatAssemblies(IEnumerable<Assembly> assemblies)
        {
            return string.Join(",", assemblies.Select(x => x.GetName().Name));
        }

        #endregion
        
    }
}