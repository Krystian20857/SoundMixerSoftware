
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.LocalSystem;
using SoundMixerSoftware.Common.Logging;
using SoundMixerSoftware.Helpers.LocalSystem;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Overlay.OverlayWindow;
using SoundMixerSoftware.Overlay.Resource;
using SoundMixerSoftware.ViewModels;
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
        public static Mutex Mutex { get; set; } = new Mutex(true, "{40F97157-0940-4877-A018-37B994816DD7}");

        #endregion
        
        #region Private Fields
        
        private readonly SimpleContainer _container = new SimpleContainer();

        #endregion
        
        #region Public Fields
        
        public LocalManager LocalManager = new LocalManager(typeof(LocalContainer));
        
        #endregion
        
        public Bootstrapper()
        {
            RegisterExceptionHandler();
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
                LocalManager.ResolveLocal();
                Initialize();
                Mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Only one instance can be running in the same time.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }
        }

        protected override object GetInstance(Type service, string key) => _container.GetInstance(service, key);

        protected override IEnumerable<object> GetAllInstances(Type service) => _container.GetAllInstances(service);

        protected override void BuildUp(object instance) => _container.BuildUp(instance);

        protected override void Configure()
        {
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