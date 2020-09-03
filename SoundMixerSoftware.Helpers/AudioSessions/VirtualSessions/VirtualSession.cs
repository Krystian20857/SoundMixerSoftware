using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Threading.Com;
using SoundMixerSoftware.Common.Utils;
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

        public SessionState State
        {
            get => Sessions.Count > 0 ? SessionState.ACTIVE : SessionState.EXITED;
            set{}
        }

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
        
        public List<AudioSessionControl> Sessions { get; } = new List<AudioSessionControl>();
        //we assume volume of all sessions is the same and host process also.
        //this part is a bit tricky... audio sessions with the same id can be(but dont have to) hosted by the same process if process exits exit event of session occurs multiple times.
        //application would think tah audio process exits multiple times. UpdateDescription methods uses State property witch is defined by count of session controls in
        //Sessions collection. it means that UpdateDescription will think that session process is still alive and try to get image which can throw exception.
        //checking is 'main' audio session process is alive each time we want to get information may be quite inefficient but at this time i cannot solve this issue by other methods.
        //***IsAlive methods uses native methods to enumerate and determine is process is alive. I decided to use this only because of that.
        //
        //USE THIS PROPERTY ONLY IF IT IS NECESSARY 
        //
        public AudioSessionControl FirstSession => Sessions.FirstOrDefault(x => ProcessUtils.IsAlive((int)x.GetProcessID));

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
                Sessions.AddRange(sessionEnum.GetSessions(sessionId));
                sessionEnum.SessionCreated += SessionEnumOnSessionCreated;
                sessionEnum.SessionExited += SessionEnumOnSessionExited;
            
                sessionEnum.VolumeChanged += SessionEnumOnVolumeChanged;
            }

            if (State == SessionState.ACTIVE)
            {
                var session = FirstSession;
                lastMute = session.SimpleAudioVolume.Mute;
                lastVolume = session.SimpleAudioVolume.Volume;
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
                    var session = FirstSession;
                    if (State == SessionState.ACTIVE && session != default)
                    {
                        using (var process = Process.GetProcessById((int) session.GetProcessID))
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
            Sessions.Add(e);
            UpdateDescription();
        }
        
        private void SessionEnumOnSessionExited(object sender, string sessionId)
        {
            if(sessionId != ID)
                return;
            RemoveSession(sender as AudioSessionControl);
            UpdateDescription();
        }
        
        private void DeviceEnumeratorOnDeviceRemoved(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            if (deviceId != DeviceId)
                return;
            Sessions.Clear();
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

            Sessions.Clear();
            Sessions.AddRange(sessionEnum.GetSessions(ID));

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
            if(State != SessionState.ACTIVE)
                return;
            try
            {
                ComThread.Invoke(() => Sessions.ForEach(x => x.SimpleAudioVolume.Volume = volume));
            }
            finally { }
        }

        internal void SetMuteInternal(bool mute)
        {
            if(State != SessionState.ACTIVE)
                return;
            try
            {
                ComThread.Invoke(() => Sessions.ForEach(x => x.SimpleAudioVolume.Mute = mute));
            }
            finally { }
        }

        internal float GetVolumeInternal()
        {
            return ComThread.Invoke(() =>
            {
                if(State != SessionState.ACTIVE)
                    return 0;
                return Sessions[0].SimpleAudioVolume.Volume;
            });
        }

        internal bool GetMuteInternal()
        {
            return ComThread.Invoke(() =>
            {
                if(State != SessionState.ACTIVE)
                    return false;
                return Sessions[0].SimpleAudioVolume.Mute;
            });
        }
        
        private void RemoveSession(AudioSessionControl session)
        {
            var instanceId = session.GetSessionInstanceIdentifier;
            var itemToRemove = Sessions.FirstOrDefault(x => x.GetSessionInstanceIdentifier == instanceId);
            if (itemToRemove == default) return;
            Sessions.Remove(itemToRemove);
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