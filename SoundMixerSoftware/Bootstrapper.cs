
using System;
using System.Collections.Generic;
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
using SoundMixerSoftware.Helpers.LocalSystem;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Utils;
using SoundMixerSoftware.ViewModels;
using SoundMixerSoftware.Views;
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

        /// <summary>
        /// Global application Tray.
        /// </summary>
        public static TaskbarIcon TaskbarIcon { get; set; }

        #endregion
        
        #region Private Fields
        
        private readonly SimpleContainer _container = new SimpleContainer();
        private StarterHelper _starter = new StarterHelper();
        private IWindowManager _windowManager = new WindowManager();
        private IEventAggregator _eventAggregator = new EventAggregator();

        #endregion
        
        #region Public Fields

        /// <summary>
        /// Current bootstrapper LocalManager.
        /// </summary>
        public LocalManager LocalManager = new LocalManager(typeof(LocalContainer));

        #endregion
        
        public Bootstrapper()
        {
            _starter.StartApplication += StarterOnStartApplication;
            _starter.BringWindowToFront += StarterOnBringWindowToFront;
            _starter.ExitApplication += StarterOnExitApplication;
            _starter.CheckInstances();
        }

        private void StarterOnExitApplication(object sender, EventArgs e)
        {
            Application.Shutdown(0x04);        
        }

        private void StarterOnBringWindowToFront(object sender, EventArgs e)
        {
            var mainWindow = MainViewModel.Instance;
            if (!mainWindow.IsActive)
                _windowManager.ShowWindowAsync(mainWindow);
            else
                (mainWindow.GetView() as MainView).WindowState = WindowState.Normal;
        }

        private void StarterOnStartApplication(object sender, EventArgs e)
        {
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            LocalManager.ResolveLocal();
            RegisterExceptionHandler();
            Initialize();
        }

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
            _container.Singleton<ButtonsViewModel>();
            
            _container.Singleton<SessionAddViewModel>();
            _container.Singleton<ButtonAddViewModel>();
            
            _container.Singleton<MainViewModel>();
            _container.Singleton<TaskbarIconViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            TaskbarIcon = Application.FindResource("TaskbarIcon") as TaskbarIcon;
            DisplayRootViewFor<MainViewModel>();
            Logger.Info("Main view started.");
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            _starter.Dispose();
            TaskbarIcon.Dispose();
            Logger.Info("App shutdown.");
            base.OnExit(sender, e);
        }

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
    }
}