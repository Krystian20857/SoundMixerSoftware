using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Caliburn.Micro;
using CommandLine;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using SoundMixerSoftware.Common.LocalSystem;
using SoundMixerSoftware.Common.Logging;
using SoundMixerSoftware.Extensibility.Loader;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Device;
using SoundMixerSoftware.Framework.LocalSystem;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Interop.Method;
using SoundMixerSoftware.Resource.Image;
using SoundMixerSoftware.Utils;
using SoundMixerSoftware.ViewModels;
using SoundMixerSoftware.Views;
using Squirrel;
using LogManager = NLog.LogManager;
using Parser = CommandLine.Parser;

namespace SoundMixerSoftware
{
    public class Bootstrapper : BootstrapperBase, ISingleInstanceApp
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
        
        #region Private Fields
        
        private readonly ExtendedContainer _container = new ExtendedContainer();
        private readonly SingleInstanceHelper _instanceHelper;

        #endregion
        
        #region Public Properties

        public IntPtr MainWindowHandle
        {
            get
            {
                if(!(IoC.Get<MainViewModel>().GetView() is Window mainWindow)) return IntPtr.Zero;
                return new WindowInteropHelper(mainWindow).Handle;
            }
        }

        public bool IsFirstRun { get; private set; }

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

        /// <summary>
        /// Command line options.
        /// </summary>
        public CmdOptions CmdOptions { get; private set; } = new CmdOptions();

        #endregion
        
        #region Events

        public event EventHandler ViewInitialized;
        
        #endregion

        #region Constructor

        public Bootstrapper()
        {
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            ParseCommandLineOptions();
            
            Instance = this;
            _instanceHelper = new SingleInstanceHelper(this);
            
            RegisterExceptionHandler();

            LocalManager.ResolveLocal();
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
            
            _container.Singleton<IWindowManager, ExtendedWindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            
            _container.Singleton<ManagerViewModel>();
            _container.Singleton<SettingsViewModel>();
            _container.Singleton<DevicesViewModel>();
            _container.Singleton<SlidersViewModel>();
            _container.Singleton<PluginViewModel>();
            _container.Singleton<ButtonsViewModel>();
            _container.Singleton<HomeViewModel>();
            _container.Singleton<HomeButtonViewModel>();
            
            _container.Singleton<SessionAddViewModel>();
            _container.Singleton<ExtensionAddViewModel>();
            _container.Singleton<ButtonAddViewModel>();

            _container.Singleton<ProfileAddViewModel>();
            _container.PerRequest<DeviceSettingsViewModel>();
            _container.PerRequest<PluginLoadViewModel>();
            _container.PerRequest<UsbManagerViewModel>();

            _container.Singleton<MainViewModel>();
            _container.Singleton<TaskbarIconViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e) => _instanceHelper.Initialize();

        protected override void OnExit(object sender, EventArgs e)
        {
            DeviceHandlerGlobal.Instance?.Dispose();
            _instanceHelper?.Dispose();
            TaskbarIcon?.Dispose();
            
            Logger.Info("App shutdown.");
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return base.SelectAssemblies().Concat(PluginLoader.LoadedPlugins.Values.Select(x => x.Assembly));
        }

        #endregion
        
        #region Implemented Methods
        
        public void Run()
        {
            Images.Initialize(LocalContainer.ImagesPath);
            PluginLoader.LoadAllPlugins();
            
            PluginLoader.ViewLoadingEvent();
            TaskbarIcon = Application.FindResource("TaskbarIcon") as TaskbarIcon;

            StartMainWindow(!ConfigHandler.ConfigStruct.Application.HideOnStartup || CmdOptions.ForceShow || IsFirstRun);

            Logger.Info("Main view started.");
                
            PluginLoader.ViewLoadedEvent();
                
            ViewInitialized?.Invoke(this, EventArgs.Empty);
        }

        public void Shutdown()
        {
            Application.Shutdown();
        }

        public void SetForeground()
        {
            var mainWindow = MainViewModel.Instance;
            if (mainWindow == null)
                return;
            var windowObject = mainWindow.GetView() as MainView;
            var hwnd = IntPtr.Zero;
            if(windowObject != null)
                hwnd = new WindowInteropHelper(windowObject).Handle;
            if (mainWindow.IsActive && hwnd != IntPtr.Zero)
            {
                windowObject.WindowState = WindowState.Normal;
            }
            if(hwnd == IntPtr.Zero)
                IoC.Get<IWindowManager>().ShowWindowAsync(mainWindow);

            User32.BringWindowToTop(hwnd);
            User32.SetForegroundWindow(hwnd);
        }
        
        #endregion
        
        #region Public Methods

        public void ReloadAssembliesForView()
        {
            AssemblySource.Instance.Clear();
            AssemblySource.Instance.AddRange(SelectAssemblies());
            AssemblySource.Instance.Refresh();
        }

        public async Task<UpdateManager> GetUpdateManager()
        {
            return await UpdateManager.GitHubUpdateManager(Constant.GITHUB_REPO_URL);
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Register unhandled exception "notifications".
        /// </summary>
        private void RegisterExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => NotifyUnhandledException(args.ExceptionObject);

            _container.OnException += (sender, args) => NotifyUnhandledException(args.ExceptionObject);
        }

        private void NotifyUnhandledException(object exceptionObject)
        {
            if (exceptionObject is Exception exception)
                ExceptionHandler.HandleException(Logger, "Unexpected exception occurs!" , exception);
            else
                Logger.Fatal("Unexpected exception occurs!", exceptionObject);
        }

        private void StartMainWindow(bool show)
        {
            var settings = new Dictionary<string, object>();
            settings.Add("showWindow", show);
            DisplayRootViewFor<MainViewModel>(settings);
        }

        private void ParseCommandLineOptions()
        {
            SquirrelAwareApp.HandleEvents(onFirstRun: () => IsFirstRun = true);
            
            var args = Environment.GetCommandLineArgs();
            Logger.Trace($"Cmd arguments: {string.Join(" ", args)}");
            Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(o =>
            {
                CmdOptions = o;
            }).WithNotParsed(o =>
            {
                Logger.Warn("Error while parsing command line arguments: ", string.Join(",", o));
            });
        }
        
        #endregion
    }
}