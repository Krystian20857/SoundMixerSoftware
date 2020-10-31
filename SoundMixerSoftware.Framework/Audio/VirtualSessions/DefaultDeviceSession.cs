using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.Observables;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Common.Utils.Audio;
using SoundMixerSoftware.Common.Utils.EnumUtils;
using SoundMixerSoftware.Framework.Utils;

namespace SoundMixerSoftware.Framework.Audio.VirtualSessions
{
    public class DefaultDeviceSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Constant

        public const string KEY = "default_device";
        public const string DEVICETYPE_KEY = "DevType";
        public const string ROLE_KEY = "Role";

        #endregion
        
        #region Private Fields

        private IAudioController _controller => SessionHandler.AudioController;

        private IDevice _device;
        private IDisposable _volumeCallback;
        private IDisposable _muteCallback;

        private bool isDefault;
        private bool isDefaultCommuninication;

        #endregion
        
        #region Implemented Properties

        public string Key { get; } = KEY;
        public string DisplayName { get; set; }
        public string ID { get; }
        public int Index { get; set; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }
        public SessionState State { get; set; } = SessionState.ACTIVE;            //always active

        public float Volume
        {
            get => (float)_device.Volume;
            set => _device.SetVolumeAsync(value);
        }

        public bool IsMute
        {
            get => _device.IsMuted;
            set => _device.SetMuteAsync(value);
        }

        #endregion
        
        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Public Properties

        public DeviceType DeviceType { get; set; }
        public Role Role { get; set; }

        #endregion
        
        #region Constructor

        public DefaultDeviceSession(Role role, DeviceType deviceType, Guid uuid)
        {
            UUID = uuid;
            DeviceType = deviceType;
            Role = role;

            ERoleUtil.GetFromRole(role, out isDefault, out isDefaultCommuninication);
            _device = _controller.GetDefaultDevice(DeviceType, role);

            _controller.AudioDeviceChanged.Subscribe(x =>
            {
                if (x.ChangedType != DeviceChangedType.DefaultChanged || x.Device.DeviceType != DeviceType)
                    return;
                var args = x as DefaultDeviceChangedArgs;
                // ReSharper disable once InvertIf
                if (args.IsDefault && isDefault || args.IsDefaultCommunications && isDefaultCommuninication)
                {
                    _device = x.Device;
                    RegisterCallbacks();
                    UpdateDescription();
                }
            });

            RegisterCallbacks();
            UpdateDescription();
        }

        public DefaultDeviceSession(int index, Role role, DeviceType deviceType, Guid uuid) : this(role, deviceType, uuid)
        {
            Index = index;
        }

        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(DEVICETYPE_KEY, DeviceType);
            result.Add(ROLE_KEY, Role);
            return result;
        }
        
        #endregion
        
        #region Private Methods

        private void RegisterCallbacks()
        {
            _volumeCallback?.Dispose();
            _volumeCallback = _device.VolumeChanged.Subscribe(x =>
            {
                VolumeChange?.Invoke(this, new VolumeChangedArgs((float)x.Volume, false, Index));
            });
               
            _muteCallback?.Dispose();
            _muteCallback = _device.MuteChanged.Subscribe(x =>
            {
                MuteChanged?.Invoke(this, new MuteChangedArgs(x.IsMuted, false, Index));
            });
        }

        private void UpdateDescription()
        {
            TaskUtil.BeginInvokeDispatcher(() => 
            {
                switch (Role)
                {
                    case Role.Multimedia:
                        switch (DeviceType)
                        {
                            case DeviceType.Playback:
                                DisplayName = "Default Speaker";
                                Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                                break;
                            case DeviceType.Capture:
                                DisplayName = "Default Microphone";
                                Image = ExtractedIcons.MicIcon.ToImageSource();
                                break;
                        }

                        break;
                    case Role.Communications:
                        switch (DeviceType)
                        {
                            case DeviceType.Playback:
                                DisplayName = "Default Communication Speaker";
                                Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                                break;
                            case DeviceType.Capture:
                                DisplayName = "Default Communication Microphone";
                                Image = ExtractedIcons.MicIcon.ToImageSource();
                                break;
                        }
                        break;
                    case Role.Console:
                        switch (DeviceType)
                        {
                            case DeviceType.Playback:
                                DisplayName = "Default Console Speaker";
                                Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                                break;
                            case DeviceType.Capture:
                                DisplayName = "Default Console Microphone";
                                Image = ExtractedIcons.MicIcon.ToImageSource();
                                break;
                        }
                        break;
                }

                DisplayName += $"({_device.FullName})";
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(Image));
            });
        }
        
        #endregion

        #region Private Events

        #endregion
        
        #region Property Change

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
            _muteCallback?.Dispose();
            _volumeCallback?.Dispose();
        }
        
        #endregion
    }

    public class DefaultDeviceSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            var roleString = (container.ContainsKey(DefaultDeviceSession.ROLE_KEY) ? container[DefaultDeviceSession.ROLE_KEY] : "")?.ToString();
            var typeString = (container.ContainsKey(DefaultDeviceSession.DEVICETYPE_KEY) ? container[DefaultDeviceSession.DEVICETYPE_KEY] : "")?.ToString();
            return new DefaultDeviceSession(index, EnumUtil.Parse(roleString, Role.Multimedia), EnumUtil.Parse(typeString, DeviceType.Playback), uuid);
        }
        
    }
}