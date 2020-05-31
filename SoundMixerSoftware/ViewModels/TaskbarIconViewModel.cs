using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using SoundMixerSoftware.Views;

namespace SoundMixerSoftware.ViewModels
{
    public class TaskbarIconViewModel
    {
        #region Private Fields
        
        private IWindowManager _windowManager = new WindowManager();
        
        #endregion
        
        #region Constructor

        public TaskbarIconViewModel()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }
        
        #endregion
        
        #region Private Events

        public void ShowWindow()
        {
            var container = IoC.Get<MainViewModel>();
            if(!container.IsActive)
                _windowManager.ShowWindow(container);
        }

        public void ExitApp()
        {
            Application.Current.Shutdown();
        }
        
        #endregion
    }
}