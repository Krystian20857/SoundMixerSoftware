using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Interop.Method;
using SoundMixerSoftware.Interop.Wrapper;
using Application = System.Windows.Application;

namespace SoundMixerSoftware
{
    /// <summary>
    /// Handles Mutex and other instances
    /// </summary>
    public class StarterHelper : IDisposable
    {
        
        #region Constant
        
        public static readonly Guid APP_UUID = new Guid("F3F46984-70BC-428B-AAC2-F8CFB4499407");            //lower case: f3f46984-70bc-428b-aac2-f8cfb4499407 <-- mutex name
        public static readonly int WM_SETFOREGROUND = User32.RegisterWindowMessage(APP_UUID.ToString());

        #endregion
        
        #region Private Fields
        
        private Mutex _mutex = new Mutex(true, APP_UUID.ToString());
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
                User32.PostMessage((IntPtr)0xFFFF, (uint)WM_SETFOREGROUND, IntPtr.Zero, IntPtr.Zero);
                ExitApplication?.Invoke(this, EventArgs.Empty);
            }

            return isAlone;
        }

        public static void RestartApp()
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = $"/C ping 127.0.0.1 -n 3 && \"{Process.GetCurrentProcess().GetFileName()}\"",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
            };
            Process.Start(startInfo);
            Application.Current.Shutdown();
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

        #region Dispose
        
        public void Dispose()
        {
            _mutex?.Close();
            _mutex?.Dispose();
        }
        
        #endregion
    }
}