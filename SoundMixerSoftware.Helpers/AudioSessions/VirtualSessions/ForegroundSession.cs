using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Threading.Com;
using SoundMixerSoftware.Helpers.Annotations;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Threading;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions
{
    public class ForegroundSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Const

        public const string KEY = "foreground-session";
        
        #endregion
        
        #region Private Fields

        private Dispatcher _dispatcher = Application.Current.Dispatcher;
        private WindowWatcher _windowWatcher = new WindowWatcher();
        
        private float lastVolume;                                               //for detecting volume/mute change
        private bool lastMute;

        #endregion
        
        #region Implemented Properties

        public string Key { get; } = KEY;
        public string DisplayName { get; set; }
        public string ID { get; } = "B0BA17A8-0CC4-458E-90F4-385794DE41FC";
        public int Index { get; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }
        public SessionState State
        {
            get => HasActiveSession ? SessionState.ACTIVE : SessionState.EXITED; 
            set { }
        }

        public float Volume
        {
            set => SetVolume(value);
            get => 0;
        }

        public bool IsMute
        {
            set => SetMute(value);
            get => false;
        }
        
        #endregion
        
        #region Public Properties

        public List<AudioSessionControl> Sessions { get; private set; } = new List<AudioSessionControl>();
        public IntPtr WindowHandle { get; private set; }
        public bool HasActiveSession => Sessions.Count > 0;

        #endregion

        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Constructor

        public ForegroundSession(int sliderIndex, Guid uuid)
        {
            UUID = uuid;
            Index = sliderIndex;

            var window = User32.GetForegroundWindow();
            var threadId = User32.GetWindowThreadProcessId(window, out var processId);
            WindowWatcherOnForegroundWindowChanged(_windowWatcher, new WindowChangedArgs(window, processId, (int)threadId));

            foreach (var sessionEnum in SessionHandler.SessionEnumerators.Values)
            {
                sessionEnum.SessionCreated += SessionEnumOnSessionCreated;
                sessionEnum.VolumeChanged += SessionEnumOnVolumeChanged;
            }

            SessionHandler.DeviceEnumerator.DeviceAdded += DeviceEnumeratorOnDeviceAdded;
            _windowWatcher.ForegroundWindowChanged += WindowWatcherOnForegroundWindowChanged;
            _windowWatcher.WindowNameChanged += WindowWatcherOnWindowNameChanged;
        }

        #endregion

        #region Implemented Methods

        public Dictionary<object, object> Save()
        {
            return new Dictionary<object, object>();
        }

        #endregion
        
        #region Private Methods

        #endregion
        
        #region Private Events
        
        private void WindowWatcherOnForegroundWindowChanged(object sender, WindowChangedArgs e)
        {
            var processId = (uint)e.ProcessId;
            var childProcesses = ProcessWrapper.GetChildProcesses(processId).ToList();

            Sessions.Clear();
            foreach (var session in SessionHandler.GetAllSessions())
            {
                var sessionProcessId = session.GetProcessID;
                if (childProcesses.Contains(sessionProcessId))
                    Sessions.Add(session);
            }
            
            WindowHandle = e.Handle;

            UpdateDescription();
        }
        
        private void SessionEnumOnSessionCreated(object sender, AudioSessionControl e)
        {
            var window = User32.GetForegroundWindow();
            var processId = e.GetProcessID;
            var windowThreadId = User32.GetWindowThreadProcessId(window, out var windowProcessId);

            if (ProcessWrapper.GetParentProcess(processId) == windowProcessId || windowProcessId == processId)
            {
                WindowHandle = window;
                Sessions.Add(e);
                UpdateDescription();
            }
        }
        
        private void DeviceEnumeratorOnDeviceAdded(object sender, EventArgs e)
        {
            var sessionEnum = SessionHandler.SessionEnumerators[sender as string];
            sessionEnum.SessionCreated += SessionEnumOnSessionCreated;
            sessionEnum.VolumeChanged += SessionEnumOnVolumeChanged;
        }

        private void SessionEnumOnVolumeChanged(object sender, Common.AudioLib.VolumeChangedArgs e)
        {
            var session = sender as AudioSessionControl;
            var sessionId = session.GetSessionIdentifier;
            if(Sessions.All(x => x.GetSessionIdentifier != sessionId))
                return;
            
            var volume = e.Volume;
            var mute = e.Mute;
            
            if (Math.Abs(volume - lastVolume) > 0.005)
            {
                VolumeChange?.Invoke(this, new VolumeChangedArgs(volume, false, Index));
                lastVolume = volume;
            }

            if (mute != lastMute)
            {
                MuteChanged?.Invoke(this, new MuteChangedArgs(e.Mute, false, Index));
                lastMute = mute;
            }
        }
        
        private void WindowWatcherOnWindowNameChanged(object sender, WindowNameChangedArgs e)
        {
            var window = e.Handle;
            if (window == WindowHandle)
            {
                UpdateDescription();
            }
        }

        #endregion
        
        #region Private Methods

        private void UpdateDescription()
        {
            _dispatcher.Invoke(() =>
            {
                if (HasActiveSession && WindowHandle != IntPtr.Zero)
                {
                    Image = (WindowWrapper.GetWindowIcon(WindowHandle) ?? ExtractedIcons.FailedIcon).ToImageSource();
                    DisplayName = $"Window: {WindowWrapper.GetWindowTitle(WindowHandle)}";
                }
                else
                {
                    DisplayName = "Focused window has not audio session.";
                    Image = ExtractedIcons.FailedIcon.ToImageSource();
                }
                OnPropertyChanged(nameof(Image));
                OnPropertyChanged(nameof(DisplayName));
            });
        }

        internal void SetVolume(float volume)
        {
            try
            {
                ComThread.BeginInvoke(() => Sessions.ForEach(x => x.SimpleAudioVolume.Volume = volume));
                if (HasActiveSession)
                    VolumeChange?.Invoke(this, new VolumeChangedArgs(volume, false, Index));
            }finally{}
        }
        
        internal void SetMute(bool mute)
        {
            try
            {
                ComThread.BeginInvoke(() => Sessions.ForEach(x => x.SimpleAudioVolume.Mute = mute));
                if (HasActiveSession)
                    MuteChanged?.Invoke(this, new MuteChangedArgs(mute, false, Index));
            }finally{}
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