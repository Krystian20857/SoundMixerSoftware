using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using SoundMixerSoftware.Common.Collection;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Interop.Method;
using SoundMixerSoftware.Interop.Threading;
using SoundMixerSoftware.Interop.Wrapper;
using SoundMixerSoftware.Resource.Image;

namespace SoundMixerSoftware.Framework.Audio.VirtualSessions
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ForegroundSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Const

        public const string KEY = "foreground-session";
        private const int SESSION_CAPACITY = 25;
        
        #endregion

        #region Private Fields

        private readonly WindowWatcher _windowWatcher = new WindowWatcher();

        private readonly ConcurrentList<IAudioSession> _sessions = new ConcurrentList<IAudioSession>(SESSION_CAPACITY); 
        private readonly Dictionary<IAudioSession, IDisposable> _volumeEvents = new Dictionary<IAudioSession, IDisposable>();
        private readonly Dictionary<IAudioSession, IDisposable> _muteEvents = new Dictionary<IAudioSession, IDisposable>();

        #endregion
        
        #region Implemented Properties

        public string Key { get; } = KEY;
        public string DisplayName { get; set; }
        public string ID { get; } = "B0BA17A8-0CC4-458E-90F4-385794DE41FC";
        public int Index { get; set; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }
        public SessionState State
        {
            get => _sessions.Count > 0 && WindowHandle != IntPtr.Zero ? SessionState.ACTIVE : SessionState.EXITED; 
            set { }
        }

        public float Volume
        {
            set => _sessions.ForEach(x => x.SetVolumeAsync(value));
            get => State == SessionState.ACTIVE ? (float)_sessions.First().Volume : 0;
        }

        public bool IsMute
        {
            set => _sessions.ForEach(x => x.SetMuteAsync(value));
            // ReSharper disable once SimplifyConditionalTernaryExpression
            get => State == SessionState.ACTIVE ? _sessions.First().IsMuted : false;
        }
        
        #endregion

        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Public Properties

        // ReSharper disable once MemberCanBePrivate.Global
        public IntPtr WindowHandle { get; protected set; }
        
        #endregion
        
        #region Constructor

        // ReSharper disable once MemberCanBePrivate.Global
        public ForegroundSession(Guid uuid)
        {
            UUID = uuid;
            
            var windowHwnd = User32.GetForegroundWindow();
            var threadId = (int)User32.GetWindowThreadProcessId(windowHwnd, out var processId);
            WindowWatcherOnForegroundWindowChanged(_windowWatcher, new WindowChangedArgs(windowHwnd, processId, threadId));
            
            _windowWatcher.ForegroundWindowChanged += WindowWatcherOnForegroundWindowChanged;
            _windowWatcher.WindowNameChanged += WindowWatcherOnWindowNameChanged;
            SessionHandler.SessionExited += SessionHandlerOnSessionExited;
            SessionHandler.SessionCreated += SessionHandlerOnSessionCreatedCallback;
        }

        public ForegroundSession(int index, Guid uuid) : this(uuid)
        {
            Index = index;
        }

        #endregion

        #region Implemented Methods

        public Dictionary<object, object> Save()
        {
            return new Dictionary<object, object>();
        }

        #endregion
        
        #region Private Methods

        private static bool IsShellWindow(IntPtr hwnd)
        {
            return hwnd == User32.GetShellWindow() || hwnd == User32.GetDesktopWindow();
        }

        private void RegisterEvents(IAudioSession session)
        {
            var volumeEvent = session.VolumeChanged.Subscribe(x =>
            {
                VolumeChange?.Invoke(this, new VolumeChangedArgs((float)x.Volume, false, Index));
            });
            
            var muteEvent = session.MuteChanged.Subscribe(x =>
            {
                MuteChanged?.Invoke(this, new MuteChangedArgs(x.IsMuted, false, Index));
            });

            _volumeEvents.Add(session, volumeEvent);
            _muteEvents.Add(session, muteEvent);
        }

        private void UnregisterEvents()
        {
            foreach(var volumeEvent in _volumeEvents.Values)
                volumeEvent.Dispose();
            _volumeEvents.Clear();
            
            foreach(var muteEvent in _muteEvents.Values)
                muteEvent.Dispose();
            _muteEvents.Clear();
        }

        private void UpdateView()
        {
            TaskUtil.BeginInvokeDispatcher(() =>
            {
                try
                {
                    if (State == SessionState.EXITED)
                    {
                        Image = Images.FailedEmbed;
                        DisplayName = "Focused window has not audio session.";
                    }
                    else
                    {
                        Image = WindowWrapper.GetWindowIcon(WindowHandle)?.ToImageSource() ?? Images.FailedEmbed;
                        DisplayName = $"Window: {WindowWrapper.GetWindowTitle(WindowHandle)}";
                    }
                }
                finally
                {
                    OnPropertyChanged(nameof(DisplayName));
                    OnPropertyChanged(nameof(Image));
                }
            });
        }

        #endregion
        
        #region Private Events
        
        private void WindowWatcherOnForegroundWindowChanged(object sender, WindowChangedArgs e)
        {
            if(IsShellWindow(e.Handle)) return;
            var processId = e.ProcessId;
            var childProcesses = ProcessWrapper.GetChildProcesses((uint)processId).ToList();
            
            _sessions.Clear();
            UnregisterEvents();
            foreach (var session in SessionHandler.GetAllSessions())
            {
                var sessionProcessId = session.ProcessId;
                // ReSharper disable once InvertIf
                if (ProcessWrapper.IsAlive(sessionProcessId) && childProcesses.Contains((uint) sessionProcessId))
                {
                    RegisterEvents(session);
                    _sessions.Add(session);
                }
            }

            WindowHandle = e.Handle;
            UpdateView();
        }
        
        private void WindowWatcherOnWindowNameChanged(object sender, WindowNameChangedArgs e)
        {
            if(e.Handle != WindowHandle)
                return;
            UpdateView();
        }
        
        private void SessionHandlerOnSessionExited(IAudioSession session)
        {
            if(_sessions.Remove(session) && State == SessionState.EXITED)
                UpdateView();
        }
        
        private void SessionHandlerOnSessionCreatedCallback(IAudioSession session)
        {
            var window = User32.GetForegroundWindow();
            var processId = session.ProcessId;
            User32.GetWindowThreadProcessId(window, out var windowProcessId);
            
            // ReSharper disable once InvertIf
            if(ProcessWrapper.GetParentProcess((uint)processId) == windowProcessId || windowProcessId == processId)
            {
                WindowHandle = window;
                RegisterEvents(session);
                _sessions.Add(session);
                UpdateView();
            }
        }
        
        #endregion
        
        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
        
        #region Dispose

        public void Dispose()
        {
            _windowWatcher?.Dispose();
            UnregisterEvents();
            
            SessionHandler.SessionExited -= SessionHandlerOnSessionExited;
            SessionHandler.SessionCreated -= SessionHandlerOnSessionCreatedCallback;
        }
        
        #endregion
    }

    public class ForegroundSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            return new ForegroundSession(index, uuid);
        }
    }
}