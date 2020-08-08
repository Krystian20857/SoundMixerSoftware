using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Threading.Com;
using SoundMixerSoftware.Helpers.Annotations;
using SoundMixerSoftware.Helpers.Utils;

namespace SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions
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
        
        private float lastVolume;                                               //for detecting volume/mute change
        private bool lastMute;
        
        #endregion
        
        #region Implemented Properties

        public string Key { get; } = KEY;
        public string DisplayName { get; set; }
        public string ID { get; }
        public int Index { get; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }
        public SessionState State { get; set; }
        public float Volume
        {
            get => GetVolumeInternal();
            set => SetVolumeInternal(value);
        }

        public bool IsMute
        {
            get => GetMuteInternal();
            set => SetMuteInternal(value);
        }
        
        #endregion
        
        #region Public Properties

        public string DeviceId { get; }
        public string RawName { get; set; }
        public string DeviceName { get; set; }
        public AudioSessionControl SessionControl { get; set; }

        #endregion
        
        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Constructor
        
        public VirtualSession(int index, string sessionId, string rawName, Guid uuid)
        {
            UUID = uuid;
            Index = index;
            RawName = rawName;
            if (string.IsNullOrEmpty(sessionId))
                return;
            ID = sessionId;
            DeviceId = Identifier.GetDeviceId(sessionId);
            if(string.IsNullOrEmpty(DeviceId))
                throw new ArgumentException($"Cannot parse device ID from: {sessionId}");
            DeviceName = ComThread.Invoke(() => SessionHandler.DeviceEnumerator.GetDeviceNull(DeviceId)?.FriendlyName ?? "Device Not Connected");
            if (SessionHandler.SessionEnumerators.ContainsKey(DeviceId))
            {
                var sessionEnum = SessionHandler.SessionEnumerators[DeviceId];
                SessionControl = sessionEnum.GetById(sessionId);
                sessionEnum.SessionCreated += SessionEnumOnSessionCreated;
                sessionEnum.SessionExited += SessionEnumOnSessionExited;
            
                sessionEnum.VolumeChanged += SessionEnumOnVolumeChanged;
            }

            if (SessionControl == null)
                State = SessionState.EXITED;
            else
            {
                State = SessionState.ACTIVE;
                lastMute = SessionControl.SimpleAudioVolume.Mute;
                lastVolume = SessionControl.SimpleAudioVolume.Volume;
            }

            UpdateDescription();

            SessionHandler.DeviceEnumerator.DeviceAdded += DeviceEnumeratorOnDeviceAdded;
            SessionHandler.DeviceEnumerator.DeviceRemoved += DeviceEnumeratorOnDeviceRemoved;
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
                    if (State == SessionState.ACTIVE && SessionControl != null)
                    {
                        using (var process = Process.GetProcessById((int) SessionControl.GetProcessID))
                            Image = process.GetMainWindowIcon().ToImageSource();
                        DisplayName = $"{RawName} - {DeviceName}";
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
        
        private void SessionEnumOnSessionCreated(object sender, AudioSessionControl e)
        {
            var sessionId = e.GetSessionIdentifier;
            if(sessionId != ID)
                return;
            SessionControl = e;
            State = SessionState.ACTIVE;
            UpdateDescription();
        }
        
        private void SessionEnumOnSessionExited(object sender, string sessionId)
        {
            if(sessionId != ID)
                return;
            SessionControl = null;
            State = SessionState.EXITED;
            UpdateDescription();
        }
        
        private void DeviceEnumeratorOnDeviceRemoved(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            if (deviceId != DeviceId)
                return;
            State = SessionState.EXITED;
            UpdateDescription();
        }

        private void DeviceEnumeratorOnDeviceAdded(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            if (deviceId != DeviceId)
                return;
            var device = SessionHandler.DeviceEnumerator.GetDeviceNull(deviceId);
            if (device == null)
                return;
            var sessionEnum = SessionHandler.SessionEnumerators[deviceId];

            sessionEnum.SessionCreated += SessionEnumOnSessionCreated;
            sessionEnum.SessionExited += SessionEnumOnSessionExited;
            sessionEnum.VolumeChanged += SessionEnumOnVolumeChanged;

            SessionControl = sessionEnum.GetById(ID);

            State = SessionState.ACTIVE;
            UpdateDescription();
        }
        
        private void SessionEnumOnVolumeChanged(object sender, Common.AudioLib.VolumeChangedArgs e)
        {
            var session = sender as AudioSessionControl;
            if (session.GetSessionIdentifier != ID)
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
        
        #endregion
        
        #region Private Methods

        internal void SetVolumeInternal(float volume)
        {
            if(State != SessionState.ACTIVE || SessionControl == null)
                return;
            try
            {
                ComThread.BeginInvoke(() => SessionControl.SimpleAudioVolume.Volume = volume);
            }
            finally { }
        }

        internal void SetMuteInternal(bool mute)
        {
            if(State != SessionState.ACTIVE || SessionControl == null)
                return;
            try
            {
                ComThread.BeginInvoke(() => SessionControl.SimpleAudioVolume.Mute = mute);
            }
            finally { }
        }

        internal float GetVolumeInternal()
        {
            if(State != SessionState.ACTIVE || SessionControl == null)
                return 0;
            return ComThread.Invoke(() => SessionControl.SimpleAudioVolume.Volume);
        }

        internal bool GetMuteInternal()
        {
            if(State != SessionState.ACTIVE || SessionControl == null)
                return false;
            return ComThread.Invoke(() => SessionControl.SimpleAudioVolume.Mute);
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