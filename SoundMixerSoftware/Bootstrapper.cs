using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using NLog;
using SoundMixerSoftware.Common.LocalSystem;
using SoundMixerSoftware.Common.Logging;
using SoundMixerSoftware.ViewModels;
using SoundMixerSoftware.Helpers.LocalSystem;
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
        
        #region Public Static Fields
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
            
            _container.PerRequest<MainViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}