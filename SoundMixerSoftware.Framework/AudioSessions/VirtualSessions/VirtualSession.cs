using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils.Audio;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Framework.AudioSessions.VirtualSessions
{
    public class VirtualSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Constant
        
        public const string KEY = "session";
        public const string SESSION_ID_KEY = "ID";
        public const string SESSION_NAME_KEY = "name";
        
        #endregion
        
        #region Private Fields

        private Dispatcher _dispatcher = Application.Current.Dispatcher;        //for property update
        private IAudioController _controller => SessionHandler.AudioController;

        private List<IAudioSession> _sessions = new List<IAudioSession>();
        private IDisposable _sessionCreatedcallback;
        private IDevice _device;

        #endregion
        
        #region Implemented Properties

        public string Key { get; } = KEY;
        public string DisplayName { get; set; }
        public string ID { get; }
        public int Index { get; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }

        public SessionState State
        {
            get => _sessions.Count > 0 ? SessionState.ACTIVE : SessionState.EXITED;
            set{}
        }

        public float Volume
        {
            get
            {
                if (State == SessionState.EXITED)
                    return 0;
                return (float)_sessions[0].Volume;
            }
            set
            {
                if(State == SessionState.EXITED)
                    return;
                _sessions.ForEach(x => x.SetVolumeAsync(value));
            }
        }

        public bool IsMute {
            get
            {
                if (State == SessionState.EXITED)
                    return false;
                return _sessions[0].IsMuted;
            }
            set
            {
                if(State == SessionState.EXITED)
                    return;
                _sessions.ForEach(x => x.SetMuteAsync(value));
            }
        }

        #endregion

        #region Public Properties

        public Guid DeviceId { get; }
        public string RawName { get;}
        public string DeviceName => _device?.FullName ?? "(Device Removed)";

        #endregion
        
        #region Private Properties

        private IAudioSession FirstSession => State == SessionState.ACTIVE ? _sessions.FirstOrDefault(x => ProcessWrapper.IsAlive(x.ProcessId)) : default;
        
        #endregion
        
        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Constructor
        
        public VirtualSession(int index, string sessionId, string rawName, Guid uuid)
        {
            Index = index;
            ID = sessionId;
            UUID = uuid;
            RawName = rawName;

            DeviceId = AudioSessionUtil.ParseDeviceId(sessionId);
            var device = _controller.GetDevice(DeviceId);
            if (device != default)
            {
                DeviceAdded(device);
            }

            SessionHandler.DeviceAddedCallback += DeviceAdded;
            SessionHandler.DeviceRemovedCallback += DeviceRemoved;
            SessionHandler.SessionExited += SessionHandlerOnSessionExited;
            
            UpdateDescription();
        }

        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(SESSION_ID_KEY, ID);
            result.Add(SESSION_NAME_KEY, RawName);
            return result;
        }
        
        #endregion
        
        #region Private Methods

        private void UpdateDescription()
        {
            _dispatcher.Invoke(() =>
            {
                try
                {
                    var session = FirstSession;
                    if (State == SessionState.ACTIVE && session != default)
                    {
                        using (var process = Process.GetProcessById(session.ProcessId))
                        {
                            Image = process.GetMainWindowIcon().ToImageSource();
                            DisplayName = $"{process.GetPreciseName()} - {DeviceName}";
                        }
                    }
                    else
                    {
                        Image = ExtractedIcons.FailedIcon.ToImageSource();
                        DisplayName = $"{RawName} - {DeviceName}(Not Active)";
                        State = SessionState.EXITED;
                    }
                }
                finally
                {
                    OnPropertyChanged(nameof(Image));
                    OnPropertyChanged(nameof(DisplayName));
                }
            });
        }

        #endregion
        
        #region Private Events
        
        private void DeviceRemoved(IDevice device)
        {
            if(device.Id != DeviceId)
                return;
            
            _sessionCreatedcallback?.Dispose();
            _sessions.Clear();
            UpdateDescription();
        }

        private void DeviceAdded(IDevice device)
        {
            if(device.Id != DeviceId)
                return;
            _device = device;
            if(_device.State != DeviceState.Active)
                return;
            var sessionController = SessionHandler.GetController(device);
            
            _sessionCreatedcallback?.Dispose();
            _sessionCreatedcallback = sessionController.SessionCreated.Subscribe(SessionCreated);
            
            var sessions = GetSessions(device, ID);
            foreach (var session in sessions)
            {
                session.VolumeChanged.Subscribe(VolumeChangedCallback);
                session.MuteChanged.Subscribe(MuteChangedCallback);
                _sessions.Add(session);
            }
            
            UpdateDescription();
        }

        private void SessionCreated(IAudioSession session)
        {
            if(session.Id != ID)
                return;
            _sessions.Add(session);
            session.VolumeChanged.Subscribe(VolumeChangedCallback);
            session.MuteChanged.Subscribe(MuteChangedCallback);
            UpdateDescription();
        }
        
        private void SessionHandlerOnSessionExited(IAudioSession session)
        {
            if(_sessions.Remove(session))
                UpdateDescription();
        }

        private void VolumeChangedCallback(SessionVolumeChangedArgs args)
        {
            VolumeChange?.Invoke(this, new VolumeChangedArgs((float)args.Volume, false, Index));
        }
        
        private void MuteChangedCallback(SessionMuteChangedArgs args)
        {
            MuteChanged?.Invoke(this, new MuteChangedArgs(args.IsMuted, false, Index));
        }
        
        #endregion
        
        #region Private Methods

        private IEnumerable<IAudioSession> GetSessions(IDevice device, string id)
        {
            return SessionHandler.GetController(device).Where(x => x.Id == id);
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
            
        }
        
        #endregion
        
    }

    public class VirtualSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            var sessionId = container.ContainsKey(VirtualSession.SESSION_ID_KEY) ? container[VirtualSession.SESSION_ID_KEY] : string.Empty;
            var sessionName = container.ContainsKey(VirtualSession.SESSION_NAME_KEY) ? container[VirtualSession.SESSION_NAME_KEY] : string.Empty;
            return new VirtualSession(index, sessionId.ToString(), sessionName.ToString(), uuid);
        }
    }
}