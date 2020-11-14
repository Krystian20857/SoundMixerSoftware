using System;
using System.Threading;
using System.Windows.Forms;
using SoundMixerSoftware.Interop.Method;
using SoundMixerSoftware.Interop.Wrapper;

namespace SoundMixerSoftware
{
    /// <summary>
    /// Handles Mutex and other instances
    /// </summary>
    public class SingleInstanceHelper : IDisposable
    {
        
        #region Constant
        
        public static readonly Guid APP_UUID = new Guid("F3F46984-70BC-428B-AAC2-F8CFB4499407");            //lower case: f3f46984-70bc-428b-aac2-f8cfb4499407 <-- mutex name
        public static readonly int WM_SETFOREGROUND = User32.RegisterWindowMessage(APP_UUID.ToString());

        #endregion
        
        #region Private Fields
        
        private Mutex _mutex = new Mutex(true, APP_UUID.ToString());
        private NativeWindowWrapper _nativeWindow = new NativeWindowWrapper();

        #endregion
        
        #region Properties

        public ISingleInstanceApp Application { get; }

        #endregion

        #region Constructor

        public SingleInstanceHelper(ISingleInstanceApp app)
        {
            Application = app;
            _nativeWindow.MessageReceived += NativeWindowOnMessageReceived;
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Check for running instances and fire events.
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            var isAlone = _mutex.WaitOne(TimeSpan.Zero, true);
            if (isAlone)
            {
                Application.Run();
                _mutex.ReleaseMutex();
            }
            else
            {
                User32.PostMessage((IntPtr)0xFFFF, (uint)WM_SETFOREGROUND, IntPtr.Zero, IntPtr.Zero);
                Application.Shutdown();
            }

            return isAlone;
        }

        #endregion

        #region Private Events

        private void NativeWindowOnMessageReceived(object sender, Message e)
        {
            if (e.Msg != WM_SETFOREGROUND)
                return;
            Application.SetForeground();
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

    public interface ISingleInstanceApp
    {
        void Run();
        void Shutdown();
        void SetForeground();
    }
}