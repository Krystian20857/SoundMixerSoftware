using System;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Windows.Forms;
using SoundMixerSoftware.Win32.Interop.Constant;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Win32.Threading
{
    public class WindowWatcher: IDisposable
    {
        #region Private Fields
        
        private IntPtr hookPtr;
        
        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when foreground window has changed.
        /// </summary>
        public event EventHandler<WindowChangedArgs> ForegroundWindowChanged;

        #endregion
        
        #region Constructor

        public WindowWatcher()
        {
            hookPtr = User32.SetWinEventHook(WIN_EVENT.EVENT_SYSTEM_FOREGROUND, WIN_EVENT.EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, WinEvent, 0, 0, WIN_EVENT.WINEVENT_OUTOFCONTEXT);
        }
        
        #endregion
        
        #region Private Events

        private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hWinEventHook != hookPtr)
                return;
            switch (eventType)
            {
                case WIN_EVENT.EVENT_SYSTEM_FOREGROUND:
                    var threadId = (int)User32.GetWindowThreadProcessId(hwnd, out var processId);
                    ForegroundWindowChanged?.Invoke(this, new WindowChangedArgs(hwnd, processId, threadId));
                    break;
            }
        }

        #endregion
        
        #region Dispose

        public void Dispose()
        {
            if(hookPtr != IntPtr.Zero)
                User32.UnhookWinEvent(hookPtr);
        }
        
        #endregion
    }

    public class WindowChangedArgs : EventArgs
    {
        public IntPtr Handle { get; set; }
        public int ProcessId { get; set; }
        public int ThreadId { get; set; }

        public WindowChangedArgs(IntPtr handle, int processId, int threadId)
        {
            Handle = handle;
            ProcessId = processId;
            ThreadId = threadId;
        }
    }
}