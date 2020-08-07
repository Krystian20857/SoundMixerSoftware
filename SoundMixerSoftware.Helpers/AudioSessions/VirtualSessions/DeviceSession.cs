using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Threading.Com;
using SoundMixerSoftware.Helpers.Annotations;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions
{
    public class DeviceSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Constant

        public const string KEY = "device";
        public const string DEVICE_ID_KEY = "id";
        public const string DEVICE_NAME_KEY = "name";
        
        #endregion
        
        #region Private Fields

        private Dispatcher _dispatcher = Application.Current.Dispatcher;
        private float lastVolume;
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
        public bool IsMute {
            get => GetMuteInternal();
            set => SetMuteInternal(value);
        }
        
        #endregion
        
        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Public Properties

        public string RawName { get; set; }
        public MMDevice Device { get; set; }

        #endregion
        
        #region Constructor

        public DeviceSession(int index, string deviceId, string rawName, Guid uuid)
        {
            Index = index;
            ID = deviceId;
            UUID = uuid;
            RawName = rawName;

            Device = ComThread.Invoke(() => SessionHandler.DeviceEnumerator.GetDeviceNull(deviceId));
            State = Device == null ? SessionState.EXITED : SessionState.ACTIVE; 
            UpdateDescription();

            if (Device != null)
            {
                ComThread.Invoke(() =>
                {
                    lastMute = Device.AudioEndpointVolume.Mute;
                    lastVolume = Device.AudioEndpointVolume.MasterVolumeLevelScalar;
                });
            }
            
            SessionHandler.DeviceEnumerator.DeviceVolumeChanged += DeviceEnumeratorOnDeviceVolumeChanged;
            SessionHandler.DeviceEnumerator.DeviceAdded += DeviceEnumeratorOnDeviceAdded;
            SessionHandler.DeviceEnumerator.DeviceRemoved += DeviceEnumeratorOnDeviceRemoved;
        }

        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(DEVICE_ID_KEY, ID);
            result.Add(DEVICE_NAME_KEY, RawName);
            return result;
        }
        
        #endregion
        
        #region Private Methods

        private void UpdateDescription()
        {
            _dispatcher.Invoke(() => 
            {
                if (State == SessionState.ACTIVE)
                {
                    DisplayName = $"{RawName}";
                    Image = IconExtractor.ExtractFromIndex(ComThread.Invoke(() => SessionHandler.DeviceEnumerator.GetDeviceById(ID).IconPath)).ToImageSource();
                }
                else
                {
                    DisplayName = $"{RawName}(Not Active)";
                    Image = ExtractedIcons.FailedIcon.ToImageSource();
                }
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(Image));
            });
        }

        internal void SetVolumeInternal(float volume)
        {
            if (State != SessionState.ACTIVE || Device == null)
                return;
            try
            {
                ComThread.BeginInvoke(() => Device.AudioEndpointVolume.MasterVolumeLevelScalar = volume);
            }finally{}
        }

        internal void SetMuteInternal(bool mute)
        {
            if (State != SessionState.ACTIVE || Device == null)
                return;
            try
            {
                ComThread.BeginInvoke(() => Device.AudioEndpointVolume.Mute = mute);
            }finally{}
        }

        internal float GetVolumeInternal()
        {
            if (State != SessionState.ACTIVE || Device == null)
                return 0;
            return ComThread.Invoke(() => Device.AudioEndpointVolume.MasterVolumeLevelScalar);
        }

        internal bool GetMuteInternal()
        {
            if (State != SessionState.ACTIVE || Device == null)
                return true;
            return ComThread.Invoke(() => Device.AudioEndpointVolume.Mute);
        }
        
        #endregion
        
        #region Private Events
        
        private void DeviceEnumeratorOnDeviceVolumeChanged(object sender, Common.AudioLib.VolumeChangedArgs e)
        {
            var deviceId = _dispatcher.Invoke(() =>
            {
                var device = sender as MMDevice;
                return string.Copy(device.ID);
            });

            if (deviceId != ID)
                return;
            var volume = e.Volume;
            if (Math.Abs(volume - lastVolume) > 0.005)
            {
                VolumeChange?.Invoke(this, new VolumeChangedArgs(volume, false, Index));
                lastVolume = volume;
            }

            var mute = e.Mute;
            if (mute != lastMute)
            {
                MuteChanged?.Invoke(this, new MuteChangedArgs(mute, false, Index));
                lastMute = mute;
            }
        }
        
        private void DeviceEnumeratorOnDeviceRemoved(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            if (State != SessionState.ACTIVE || deviceId != ID)
                return;
            State = SessionState.EXITED;
            UpdateDescription();
        }

        private void DeviceEnumeratorOnDeviceAdded(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            if (State != SessionState.EXITED || deviceId != ID)
                return;
            State = SessionState.ACTIVE;
            UpdateDescription();
            Device = ComThread.Invoke(() => SessionHandler.DeviceEnumerator.GetDeviceNull(deviceId));
        }
        
        #endregion
        
        #region Property Change

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }

    public class DeviceSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            var id = container.ContainsKey(DeviceSession.DEVICE_ID_KEY) ? container[DeviceSession.DEVICE_ID_KEY] : string.Empty;
            var deviceName = container.ContainsKey(DeviceSession.DEVICE_NAME_KEY) ? container[DeviceSession.DEVICE_NAME_KEY] : string.Empty;
            return new DeviceSession(index, id.ToString(), deviceName.ToString(), uuid);
        }
    }
}