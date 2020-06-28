using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SoundMixerSoftware.Win32.Utils;
using SoundMixerSoftware.Win32.Win32;

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

        public event EventHandler BringWindowToFront;
        public event EventHandler StartApplication;
        public event EventHandler ExitApplciation;
        
        #endregion
        
        #region Constructor

        public StarterHelper()
        {
            _nativeWindow.MessageReceived += NativeWindowOnMessageReceived;
        }
        
        
        #endregion
        
        #region Public Methods

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
                NativeMethods.PostMessage((IntPtr)0xFFFF, WM_SETFOREGROUND, IntPtr.Zero, IntPtr.Zero);
                ExitApplciation?.Invoke(this, EventArgs.Empty);
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