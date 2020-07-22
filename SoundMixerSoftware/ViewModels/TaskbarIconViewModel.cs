

using System.Windows;
using System.Windows.Input;

namespace SoundMixerSoftware.ViewModels
{
    public class TaskbarIconViewModel
    {
        #region Private Fields
        
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
            Bootstrapper.Instance.BringToFront();
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