using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using SoundMixerSoftware.Helpers.LocalSystem;
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
            var mainWindow = MainViewModel.Instance;
            if(!mainWindow.IsActive)
                _windowManager.ShowWindowAsync(mainWindow);
            else
                (mainWindow.GetView() as MainView).WindowState = WindowState.Normal;
        }

        public void ExitApp()
        {
            Application.Current.Shutdown();
        }

        public void RestartApp()
        {
            StarterHelper.RestartApp();
        }

        #endregion
    }
}