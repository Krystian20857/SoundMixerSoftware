
using System;
using System.Collections.Generic;
using System.Windows;
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
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Static Properties

        public static TaskbarIcon TaskbarIcon { get; set; }
        

        #endregion
        
        #region Private Fields
        
        private readonly SimpleContainer _container = new SimpleContainer();
        private StarterHelper _starter = new StarterHelper();
        private IWindowManager _windowManager = new WindowManager();

        #endregion
        
        #region Public Fields

        public LocalManager LocalManager = new LocalManager(typeof(LocalContainer));

        #endregion
        
        public Bootstrapper()
        {
            _starter.StartApplication += StarterOnStartApplication;
            _starter.BringWindowToFront += StarterOnBringWindowToFront;
            _starter.ExitApplciation += StarterOnExitApplication;
            _starter.CheckInstances();
        }

        private void StarterOnExitApplication(object sender, EventArgs e)
        {
           Application.Shutdown(0x04);
        }

        private void StarterOnBringWindowToFront(object sender, EventArgs e)
        {
            var mainWindow = IoC.Get<MainViewModel>();
            var view = mainWindow.GetView() as MainView;
            if (!mainWindow.IsActive)
                _windowManager.ShowWindowAsync(mainWindow);
            else
                view.WindowState = WindowState.Normal;
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
            
            _container.PerRequest<ManagerViewModel>();
            _container.PerRequest<SettingsViewModel>();
            _container.PerRequest<DevicesViewModel>();
            _container.PerRequest<SlidersViewModel>();
            _container.PerRequest<ButtonsViewModel>();
            
            _container.PerRequest<SessionAddViewModel>();
            
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
            TaskbarIcon.Dispose();
            Logger.Info("App shutdown.");
            base.OnExit(sender, e);
        }

        private void RegisterExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exceptionObject = e.ExceptionObject;
            if (exceptionObject is Exception exception)
                ExceptionHandler.HandleException(Logger, exception);
            else
                Logger.Error(exceptionObject.ToString());
            Logger.Error("UNHANDLED EXCEPTIONS WILL CRASH ENTIRE APPLICATION!");
        }
    }
}