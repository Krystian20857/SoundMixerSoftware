
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using SoundMixerSoftware.Common.LocalSystem;
using SoundMixerSoftware.Common.Logging;
using SoundMixerSoftware.Extensibility.Loader;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.LocalSystem;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Utils;
using SoundMixerSoftware.ViewModels;
using SoundMixerSoftware.Views;
using SoundMixerSoftware.Win32.Interop.Method;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware
{
    public class Bootstrapper : BootstrapperBase
    {
        #region Logger

        /// <summary>
        /// Current Class Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Static Properties

        public static Bootstrapper Instance { get; private set; }
#if DEBUG
        //running small node server with hosted files and github-like release files.
        public const string RELEASES_URL = "http://localhost:3000/debug_releases.json";                
#else
        public const string RELEASES_URL = "https://api.github.com/repos/Krystian20857/SoundMixerSoftware/releases";
#endif

        #endregion
        
        #region Public Properties

        public IntPtr MainWindowHandle
        {
            get
            {
                var mainWindow = Application.Current.MainWindow;
                if(mainWindow == null) return IntPtr.Zero;
                return new WindowInteropHelper(mainWindow).Handle;
            }
        } 

        /// <summary>
        /// Global application Tray.
        /// </summary>
        public TaskbarIcon TaskbarIcon { get; set; }
        
        /// <summary>
        /// Plugin loader instance.
        /// </summary>
        public PluginLoader PluginLoader { get; } = new PluginLoader(LocalContainer.PluginFolder, LocalContainer.PluginCache);

        /// <summary>
        /// Current bootstrapper LocalManager.
        /// </summary>
        public LocalManager LocalManager { get; } = new LocalManager(typeof(LocalContainer));

        public Updater.Updater Updater { get; protected set; }

        #endregion
        
        #region Private Fields
        
        private readonly ExtendedContainer _container = new ExtendedContainer();
        private StarterHelper _starter = new StarterHelper();
        private IWindowManager _windowManager = new WindowManager();

        #endregion
        
        #region Constructor

        public Bootstrapper()
        {
            Instance = this;
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            RegisterExceptionHandler();
            
            _starter.StartApplication += (sender, args) =>
            {
                Updater = new Updater.Updater(Assembly.GetExecutingAssembly().GetName().Version, RELEASES_URL, LocalContainer.InstallerDownloadCache, starter =>
                {
                    _starter.Dispose();
                    Application.Current.Shutdown();
                });
                PluginLoader.ViewLoadingEvent();
                IoC.Get<TaskbarIconViewModel>();
                TaskbarIcon = Application.FindResource("TaskbarIcon") as TaskbarIcon;
                if (ConfigHandler.ConfigStruct.Application.HideOnStartup)
                    IoC.Get<MainViewModel>();
                else
                    DisplayRootViewFor<MainViewModel>();
                Logger.Info("Main view started.");
                PluginLoader.ViewLoadedEvent();
            };

            _starter.BringWindowToFront += (sender, args) => BringToFront();

            _starter.ExitApplication += (sender, args) => Application.Shutdown(0x04);

            LocalManager.ResolveLocal();
            PluginLoader.LoadAllPlugins();
            Initialize();
        }
        
        #endregion
        
        #region Overriden Methods

        protected override object GetInstance(Type service, string key) => _container.GetInstance(service, key);

        protected override IEnumerable<object> GetAllInstances(Type service) => _container.GetAllInstances(service);

        protected override void BuildUp(object instance) => _container.BuildUp(instance);

        protected override void Configure()
        {
            ParserHelper.ConfigureShortCuts();
            
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            
            _container.Singleton<ManagerViewModel>();
            _container.Singleton<SettingsViewModel>();
            _container.Singleton<DevicesViewModel>();
            _container.Singleton<SlidersViewModel>();
            _container.Singleton<PluginViewModel>();
            _container.Singleton<ButtonsViewModel>();
            
            _container.Singleton<SessionAddViewModel>();
            _container.Singleton<ExtensionAddViewModel>();
            _container.Singleton<ButtonAddViewModel>();

            _container.Singleton<ProfileAddViewModel>();
            _container.PerRequest<DeviceSettingsViewModel>();
            _container.PerRequest<PluginLoadViewModel>();
            _container.PerRequest<UsbManagerViewModel>();
            _container.PerRequest<UpdateViewModel>();

            _container.Singleton<MainViewModel>();
            _container.Singleton<TaskbarIconViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            _starter.CheckInstances();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            DeviceHandlerGlobal.Instance?.Dispose();
            _starter?.Dispose();
            TaskbarIcon?.Dispose();
            Logger.Info("App shutdown.");
            base.OnExit(sender, e);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return base.SelectAssemblies().Concat(PluginLoader.LoadedPlugins.Values.Select(x => x.Assembly));
        }

        #endregion
        
        #region Public Methods

        public void ReloadAssembliesForView()
        {
            AssemblySource.Instance.Clear();
            AssemblySource.Instance.AddRange(SelectAssemblies());
            AssemblySource.Instance.Refresh();
        }

        public void BringToFront()
        {
            var mainWindow = MainViewModel.Instance;
            if (mainWindow == null)
                return;
            if (mainWindow.IsActive)
                (mainWindow.GetView() as MainView).WindowState = WindowState.Normal;
            else
                _windowManager.ShowWindowAsync(mainWindow);
            
            User32.BringWindowToTop(MainWindowHandle);
        }
        

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Register unhandled exception "notifications".
        /// </summary>
        private void RegisterExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => NotifyUnhandledException(args.ExceptionObject);

            Application.Current.DispatcherUnhandledException += (sender, args) => NotifyUnhandledException(args.Exception);

            _container.OnException += (sender, args) => NotifyUnhandledException(args.ExceptionObject);
        }

        private void NotifyUnhandledException(object exceptionObject)
        {
            if (exceptionObject is Exception exception)
                ExceptionHandler.HandleException(Logger, "Unexpected exception occurs!" , exception);
            else
                Logger.Error(exceptionObject);
            Logger.Error("UNHANDLED EXCEPTIONS WILL CRASH ENTIRE APPLICATION!");
        }

        #endregion
    }
}