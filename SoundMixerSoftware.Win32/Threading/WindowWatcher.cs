﻿using System;
 using NLog;
 using SoundMixerSoftware.Win32.Interop.Constant;
using SoundMixerSoftware.Win32.Interop.Method;
 using SoundMixerSoftware.Win32.Wrapper;

 namespace SoundMixerSoftware.Win32.Threading
{
    public class WindowWatcher: IDisposable
    {
        #region Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields
        
        private readonly IntPtr hookPtr;
        private readonly User32.WinEventDelegate _winEventDelegate;
        
        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when foreground window has changed.
        /// </summary>
        public event EventHandler<WindowChangedArgs> ForegroundWindowChanged;
        /// <summary>
        /// Occurs when window name has changed.
        /// </summary>
        public event EventHandler<WindowNameChangedArgs> WindowNameChanged;

        #endregion
        
        #region Constructor

        public WindowWatcher()
        {
            _winEventDelegate = new User32.WinEventDelegate(WinEvent);   
            //fixes null exception in application message loop. turning off code optimization will work either.
            GC.KeepAlive(_winEventDelegate);                                
            
            hookPtr = User32.SetWinEventHook(WIN_EVENT.EVENT_OBJECT_FOCUS, WIN_EVENT.EVENT_OBJECT_NAMECHANGE,
                IntPtr.Zero, _winEventDelegate, 0, 0, WIN_EVENT.WINEVENT_OUTOFCONTEXT);
        }
        
        #endregion
        
        #region Private Events

        private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if(hWinEventHook != hookPtr)
                return;
            var threadId = (int) User32.GetWindowThreadProcessId(hwnd, out var processId);
            switch (eventType)
            {
                case WIN_EVENT.EVENT_OBJECT_FOCUS:
                    ForegroundWindowChanged?.Invoke(this, new WindowChangedArgs(hwnd, processId, threadId));
                    break;
                case WIN_EVENT.EVENT_OBJECT_NAMECHANGE:
                    WindowNameChanged?.Invoke(this, new WindowNameChangedArgs(hwnd, processId, threadId, WindowWrapper.GetWindowTitle(hwnd)));
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

    public class WindowNameChangedArgs : WindowChangedArgs
    {
        public string WindowName { get; set; }

        public WindowNameChangedArgs(IntPtr handle, int processId, int threadId) : base(handle, processId, threadId)
        {
        }

        public WindowNameChangedArgs(IntPtr handle, int processId, int threadId, string windowName) : base(handle, processId, threadId)
        {
            WindowName = windowName;
        }
    }
}