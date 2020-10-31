using System;
using System.Collections.Generic;
using System.Windows.Media;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.Observables;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils.EnumUtils;
using SoundMixerSoftware.Framework.Audio;
using SoundMixerSoftware.Framework.Device;
using SoundMixerSoftware.Framework.Overlay;
using SoundMixerSoftware.Framework.Profile;

namespace SoundMixerSoftware.Framework.Buttons.Functions
{
    public class MuteFunction : IButtonFunction
    {
        #region Constant

        public const string MUTE_TASK_KEY = "mute";
        public const string SLIDER_INDEX = "slider_index";
        
        #endregion
        
        #region Private Fields

        private IAudioController _controller = SessionHandler.AudioController;
        private IDevice _device;
        private IDisposable _defaultVolumeCallback;

        private string _name;

        private bool _lastSliderMute;
        
        #endregion

        #region Implemented Properties

        public string Name
        {
            get
            {
                switch (MuteTask)
                {
                    case MuteTask.MuteDefaultSpeaker:
                        _name = $"Default Speaker Mute";
                        break;
                    case MuteTask.MuteDefaultMic:
                        _name = $"Default Microphone Mute";
                        break;
                    case MuteTask.MuteSlider:
                        if (SliderIndex >= ProfileHandler.SelectedProfile.SliderCount)
                            _name = $"Slider Mute: slider out of size";
                        else
                            _name = $"Slider Mute: {ProfileHandler.SelectedProfile.Sliders[SliderIndex].Name}";
                        break;
                }
                return _name;
            }
            set => _name = value;
        }

        public string Key { get; } = "mute_func"; 
        public int Index { get; set; }
        public Guid UUID { get; set; }
        public ImageSource Image { get; set; } = Resource.MuteIcon.ToImageSource();
        
        #endregion
        
        #region Public Properties

        public int SliderIndex { get; }
        public MuteTask MuteTask { get; }
        public DeviceType DeviceType { get; set; }

        #endregion

        #region Constructor

        protected MuteFunction(MuteTask task)
        {
            MuteTask = task;
            
            switch (task)
            {
                case MuteTask.MuteDefaultSpeaker:
                    DeviceType = DeviceType.Playback;
                    DefaultDeviceChanged(new DefaultDeviceChangedArgs(_controller.DefaultPlaybackDevice));
                    _device = _controller.DefaultPlaybackDevice;
                    break;
                case MuteTask.MuteDefaultMic:
                    DeviceType = DeviceType.Capture;
                    DefaultDeviceChanged(new DefaultDeviceChangedArgs(_controller.DefaultCaptureDevice));
                    _device = _controller.DefaultCaptureDevice;
                    break;
            }

            switch (task)
            {
                case MuteTask.MuteDefaultMic:
                case MuteTask.MuteDefaultSpeaker:
                    _controller.AudioDeviceChanged.Subscribe(x =>
                    {
                        if (x.ChangedType != DeviceChangedType.DefaultChanged)
                            return;
                        DefaultDeviceChanged(x as DefaultDeviceChangedArgs);
                    });
                    break;
                case MuteTask.MuteSlider:
                    SessionHandler.SessionMuteChanged += (sender, args) =>
                    {
                        var index = args.Index;
                        if(index != SliderIndex)
                            return;
                        _lastSliderMute = args.Mute;
                        DeviceNotifier.LightButton(unchecked( (byte)Index ), _lastSliderMute); 
                    };
                    break;
            }
        }

        public MuteFunction(int sliderIndex, Guid uuid) : this(MuteTask.MuteSlider)
        {
            SliderIndex = sliderIndex;
            UUID = uuid;
        }
        
        public MuteFunction(MuteTask muteTask, Guid uuid): this(muteTask)
        {
            UUID = uuid;
        }

        public MuteFunction(int index, int sliderIndex, Guid uuid) : this(sliderIndex, uuid)
        {
            Index = index;
        }
        
        public MuteFunction(int index, MuteTask muteTask, Guid uuid) : this(muteTask, uuid)
        {
            Index = index;
        }

        #endregion
        
        #region Implemented Methods

        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(MUTE_TASK_KEY, MuteTask);
            switch (MuteTask)
            {
                case MuteTask.MuteSlider:
                    result.Add(SLIDER_INDEX, SliderIndex);
                    break;
            }
            return result;
        }

        public void ButtonKeyDown(int index)
        {
            switch (MuteTask)
            {
                case MuteTask.MuteDefaultSpeaker:
                case MuteTask.MuteDefaultMic:
                    _device.ToggleMuteAsync().ContinueWith(x => OverlayHandler.ShowMute(_device.IsMuted));
                    break;
                case MuteTask.MuteSlider:
                    var mute = !_lastSliderMute;
                    SessionHandler.SetMute(SliderIndex, mute, true);
                    OverlayHandler.ShowMute(mute);
                    break;
            }
        }

        public void ButtonKeyUp(int index)
        {
            
        }
        
        #endregion
        
        #region Private Methods

        private void DefaultDeviceChanged(DefaultDeviceChangedArgs x)
        {
            var device = x.Device;
            if (device.IsDefaultDevice && device.DeviceType == DeviceType)
            {
                _device = device;
                _defaultVolumeCallback?.Dispose();
                _defaultVolumeCallback = _device.MuteChanged.Subscribe(y =>
                {
                    DeviceNotifier.LightButton(unchecked((byte) Index), y.IsMuted);
                });
            }
        }
        
        #endregion 
        
        #region Destructor

        ~MuteFunction()
        {
            _defaultVolumeCallback?.Dispose();
        }
        
        #endregion
    }

    public class MuteFunctionCreator : IButtonCreator
    {
        #region Implemented Methods
        
        public IButtonFunction CreateButton(int index, Dictionary<object, object> container, Guid uuid)
        {
            var muteTask = container.ContainsKey(MuteFunction.MUTE_TASK_KEY) ? EnumUtil.Parse<MuteTask>(container[MuteFunction.MUTE_TASK_KEY].ToString()) : MuteTask.MuteDefaultSpeaker;
            switch (muteTask)
            {
                case MuteTask.MuteSlider:
                    var sliderIndex = int.TryParse(container[MuteFunction.SLIDER_INDEX].ToString(), out var result) ? result : 0;
                    return new MuteFunction(index, sliderIndex, uuid);
                default:
                    return new MuteFunction(index, muteTask, uuid);
            }
        }
        
        #endregion
    }

    public enum MuteTask
    {
        [ValueName("Mute/Unmute Default Speaker")]MuteDefaultSpeaker,
        [ValueName("Mute/Unmute Default Microphone")]MuteDefaultMic,
        [ValueName("Mute/Unmute Slider")]MuteSlider
    }
}