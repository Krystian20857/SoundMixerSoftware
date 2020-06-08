
using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.LocalSystem;
using SoundMixerSoftware.Common.Logging;
using SoundMixerSoftware.Helpers.LocalSystem;
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

        #endregion
        
        #region Private Fields
        
        private readonly SimpleContainer _container = new SimpleContainer();

        #endregion
        
        #region Public Fields
        
        public LocalManager LocalManager = new LocalManager(typeof(LocalContainer));
        
        #endregion
        
        public Bootstrapper()
        { 
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            LocalManager.ResolveLocal();

            Initialize();
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
    }
}