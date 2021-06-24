using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.Observables;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Interop.Wrapper;
using SoundMixerSoftware.Resource.Image;

namespace SoundMixerSoftware.Framework.Audio.VirtualSessions
{
    public class DeviceSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Constant

        public const string KEY = "device";
        public const string DEVICE_ID_KEY = "id";
        public const string DEVICE_NAME_KEY = "name";
        
        #endregion
        
        #region Private Fields
        
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IAudioController _controller => SessionHandler.AudioController;
        
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string _rawName;
        private IDevice _device;

        private IDisposable _volumeCallback;
        private IDisposable _muteCallback;

        #endregion
        
        #region Implemented Properties

        public string Key { get; } = KEY;
        public string DisplayName { get; set; }
        public string ID { get; }
        public int Index { get; set; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }
        public SessionState State
        {
            get => _device == default ? SessionState.EXITED : SessionState.ACTIVE;
            set { }
        }

        public float Volume
        {
            get => (float)(_device?.Volume ?? -1);
            set => _device?.SetVolumeAsync(value);
        }
        public bool IsMute {
            get => _device?.IsMuted ?? false;
            set => _device?.SetMuteAsync(value);
        }
        
        #endregion
        
        #region Private Properties
        private string Name => _device?.FullName ?? _rawName;

        #endregion
        
        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Public Properties

        // ReSharper disable once MemberCanBePrivate.Global
        public Guid DeviceID { get; }

        #endregion
        
        #region Constructor

        // ReSharper disable once MemberCanBePrivate.Global
        public DeviceSession(Guid deviceId, string rawName, Guid uuid)
        {
            DeviceID = deviceId;
            ID = deviceId.ToString();
            UUID = uuid;
            _rawName = rawName;

            _device = _controller.GetDevice(deviceId, DeviceState.Active);
            
            if (State == SessionState.ACTIVE)
            {
                RegisterCallbacks();
            }

            SessionHandler.DeviceAddedCallback += DeviceAdded;
            SessionHandler.DeviceRemovedCallback += DeviceRemoved;

            UpdateDescription();
        }

        public DeviceSession(int index, Guid deviceId, string rawName, Guid uuid) : this(deviceId, rawName, uuid)
        {
            Index = index;
        }

        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(DEVICE_ID_KEY, ID);
            result.Add(DEVICE_NAME_KEY, Name);
            return result;
        }
        
        #endregion
        
        #region Private Methods

        private void UpdateDescription()
        {
            TaskUtil.BeginInvokeDispatcher(() =>
            {
                if (State == SessionState.ACTIVE)
                {
                    DisplayName = $"{Name}";
                    Image = IconExtractor.ExtractFromIndex(_device?.IconPath).ToImageSource();
                }
                else if (_controller.GetDevice(DeviceID) == null)
                {
                    DisplayName = $"{Name}(Not Connected)";
                    Image = Images.FailedEmbed;
                }
                else
                {
                    DisplayName = $"{Name}(Not Active)";
                    Image = Images.FailedEmbed;
                }
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(Image));
            });
        }

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

        #endregion
        
        #region Private Events

        private void DeviceAdded(IDevice device)
        {            
            if(device.Id != DeviceID)
                return;
            _device = device;
            RegisterCallbacks();
            UpdateDescription();
        }

        private void DeviceRemoved(IDevice device)
        {
            if(device.Id != DeviceID)
                return;
            _device = null;
            UpdateDescription();
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

        #region Dispose
        
        public void Dispose()
        {
            _muteCallback?.Dispose();
            _volumeCallback?.Dispose();
            SessionHandler.DeviceAddedCallback -= DeviceAdded;
            SessionHandler.DeviceRemovedCallback -= DeviceRemoved;
        }
        
        #endregion
    }

    public class DeviceSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            var id = container.ContainsKey(DeviceSession.DEVICE_ID_KEY) ? container[DeviceSession.DEVICE_ID_KEY] : string.Empty;
            var deviceName = container.ContainsKey(DeviceSession.DEVICE_NAME_KEY) ? container[DeviceSession.DEVICE_NAME_KEY] : string.Empty;
            var deviceId = Guid.TryParse(id.ToString(), out var var1) ? var1 : Guid.Empty;
            return new DeviceSession(index, deviceId, deviceName.ToString(), uuid);
        }
    }
}