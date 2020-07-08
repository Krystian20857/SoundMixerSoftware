using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SoundMixerSoftware.Win32.Interop;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware
{
    /// <summary>
    /// Handles Mutex and other instances
    /// </summary>
    public class StarterHelper
    {
        
        #region Constant
        
        public const int WM_SETFOREGROUND = 0xDD64;
        
        #endregion
        
        #region Private Fields
        
        private Mutex _mutex = new Mutex(true, "F3F46984-70BC-428B-AAC2-F8CFB4499407");
        private NativeWindowWrapper _nativeWindow = new NativeWindowWrapper();
        
        #endregion
        
        #region Public Events

        /// <summary>
        /// Occurs when application instance need to be set as foreground window.
        /// </summary>
        public event EventHandler BringWindowToFront;
        /// <summary>
        /// Occurs when any of applications instances are not running and can be safely started.  
        /// </summary>
        public event EventHandler StartApplication;
        /// <summary>
        /// Occurs when another instance of application is running and started application need to be exited.
        /// </summary>
        public event EventHandler ExitApplication;
        
        #endregion
        
        #region Constructor

        public StarterHelper()
        {
            _nativeWindow.MessageReceived += NativeWindowOnMessageReceived;
        }
        
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Check for running instances and fire events.
        /// </summary>
        /// <returns></returns>
        public bool CheckInstances()
        {
            var isAlone = _mutex.WaitOne(TimeSpan.Zero, true);
            if (isAlone)
            {
                StartApplication?.Invoke(this, EventArgs.Empty);
                _mutex.ReleaseMutex();
            }
            else
            {
                User32.PostMessage((IntPtr)0xFFFF, WM_SETFOREGROUND, IntPtr.Zero, IntPtr.Zero);
                ExitApplication?.Invoke(this, EventArgs.Empty);
            }

            return isAlone;
        }
        
        #endregion

        #region Private Events

        private void NativeWindowOnMessageReceived(object sender, Message e)
        {
            if (e.Msg != WM_SETFOREGROUND)
                return;
            BringWindowToFront?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion
    }
}