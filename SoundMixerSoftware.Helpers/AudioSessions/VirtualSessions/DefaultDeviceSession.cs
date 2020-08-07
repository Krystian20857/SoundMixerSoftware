using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Threading.Com;
using SoundMixerSoftware.Common.Utils.EnumUtils;
using SoundMixerSoftware.Helpers.Annotations;
using SoundMixerSoftware.Helpers.Utils;

namespace SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions
{
    public class DefaultDeviceSession : IVirtualSession
    {
        #region Constant

        public const string KEY = "default-device";
        public const string ID_KEY = "id";
        public const string DATAFLOW_KEY = "dataflow";
        public const string MODE_KEY = "mode";
        
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
        public SessionState State { get; set; } = SessionState.ACTIVE;            //always active

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
        
        public MMDevice Device { get; set; }
        public string DeviceId => ComThread.Invoke(() => Device.ID);

        public DefaultDeviceMode Mode { get; set; }
        public DataFlow DataFlow { get; set; }
        public Role ERole => Mode == DefaultDeviceMode.DEFAULT_COMMUNICATION ? Role.Communications : Role.Multimedia;

        #endregion
        
        #region Constructor

        public DefaultDeviceSession(int index, DefaultDeviceMode mode, DataFlow dataFlow, Guid uuid)
        {
            Index = index;
            ID = mode.CreateStringUUID(dataFlow);
            UUID = uuid;

            Mode = mode;
            DataFlow = dataFlow;
            
            UpdateDescription();

            Device = ComThread.Invoke(() => SessionHandler.DeviceEnumerator.GetDefaultEndpoint(DataFlow, ERole));
            
            SessionHandler.DeviceEnumerator.DeviceVolumeChanged += DeviceEnumeratorOnDeviceVolumeChanged;
            SessionHandler.DeviceEnumerator.DefaultDeviceChange += DeviceEnumeratorOnDefaultDeviceChange;
        }

        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(DATAFLOW_KEY, DataFlow);
            result.Add(MODE_KEY, Mode);
            return result;
        }
        
        #endregion
        
        #region Private Methods

        private void UpdateDescription()
        {
            _dispatcher.Invoke(() => 
            {
                switch (Mode)
                {
                    case DefaultDeviceMode.DEFAULT_MULTIMEDIA:
                        switch (DataFlow)
                        {
                            case DataFlow.Render:
                                DisplayName = "Default Speaker";
                                Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                                break;
                            case DataFlow.Capture:
                                DisplayName = "Default Microphone";
                                Image = ExtractedIcons.MicIcon.ToImageSource();
                                break;
                            case DataFlow.All:
                                DisplayName = "Default Speaker/Microphone";
                                Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                                break;
                        }
                        break;
                    case DefaultDeviceMode.DEFAULT_COMMUNICATION:
                        switch (DataFlow)
                        {
                            case DataFlow.Render:
                                DisplayName = "Default Communication Speaker";
                                Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                                break;
                            case DataFlow.Capture:
                                DisplayName = "Default Communication Microphone";
                                Image = ExtractedIcons.MicIcon.ToImageSource();
                                break;
                            case DataFlow.All:
                                DisplayName = "Default Communication Speaker/Microphone";
                                Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                                break;
                        }
                        break;
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

            if (deviceId != DeviceId)
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
        
        private void DeviceEnumeratorOnDefaultDeviceChange(object sender, DefaultDeviceChangedArgs e)
        {
            var deviceId = sender as string;
            if (e.Role != ERole || e.DataFlow != DataFlow)
                return;
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

    public class DefaultDeviceSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            //var id = container.ContainsKey(DefaultDeviceSession.ID_KEY) ? container[DefaultDeviceSession.ID_KEY] : default;
            var dataFlowString = (container.ContainsKey(DefaultDeviceSession.DATAFLOW_KEY) ? container[DefaultDeviceSession.DATAFLOW_KEY] : default)?.ToString();
            var modeString = (container.ContainsKey(DefaultDeviceSession.MODE_KEY) ? container[DefaultDeviceSession.MODE_KEY] : default)?.ToString();
            var mode = EnumUtils.Parse<DefaultDeviceMode>(modeString);
            var dataFlow = EnumUtils.Parse<DataFlow>(dataFlowString);
            return new DefaultDeviceSession(index,mode ,dataFlow , uuid);
        }
    }
}