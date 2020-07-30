
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using SoundMixerSoftware.Common.LocalSystem;
using SoundMixerSoftware.Common.Logging;
using SoundMixerSoftware.Extensibility.Loader;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.LocalSystem;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Utils;
using SoundMixerSoftware.ViewModels;
using SoundMixerSoftware.Views;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Wrapper;
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

        #endregion
        
        #region Public Properties

        public IntPtr MainWindowHandle => new WindowInteropHelper(Application.Current.MainWindow ?? throw new NullReferenceException("Application has not associated window.")).Handle;

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

        #endregion
        
        #region Private Fields
        
        private readonly SimpleContainer _container = new SimpleContainer();
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
                PluginLoader.ViewLoadingEvent();
                TaskbarIcon = Application.FindResource("TaskbarIcon") as TaskbarIcon;
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
            _container.Singleton<ButtonAddViewModel>();

            _container.PerRequest<PluginLoadViewModel>();
            
            _container.Singleton<MainViewModel>();
            _container.Singleton<TaskbarIconViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            _starter.CheckInstances();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            DeviceHandlerGlobal.Instance.Dispose();
            _starter.Dispose();
            TaskbarIcon.Dispose();
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
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exceptionObject = args.ExceptionObject;
                if (exceptionObject is Exception exception)
                    ExceptionHandler.HandleException(Logger, "Unexpected exception occurs!!!" ,exception);
                else
                    Logger.Error(exceptionObject.ToString());
                Logger.Error("UNHANDLED EXCEPTIONS WILL CRASH ENTIRE APPLICATION!");
            };
        }
        
        #endregion
    }
}