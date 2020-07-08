using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils.EnumUtils;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.Overlay;
using SoundMixerSoftware.Helpers.Profile;
using VolumeChangedArgs = SoundMixerSoftware.Common.AudioLib.VolumeChangedArgs;

namespace SoundMixerSoftware.Helpers.Buttons.Functions
{
    public class MuteFunction : IButton
    {
        #region Constant

        public const string MUTE_TASK_KEY = "mute";
        public const string SLIDER_INDEX = "slider_index";
        
        #endregion
        
        #region Static Fields

        private static IDictionary<string, bool> _lastMute =  new Dictionary<string, bool>();
        private static List<MuteFunction> _speakerMute = new List<MuteFunction>();
        private static List<MuteFunction> _micMute = new List<MuteFunction>();
        private static IDictionary<int, bool> _sliderMute = new Dictionary<int, bool>();

        #endregion
        
        #region Static Properties

        public static Dictionary<int, List<int>> SliderMute { get; } = new Dictionary<int, List<int>>(); // int: index of slider, List<int>: indexes of assigned buttons

        #endregion
        
        #region Private Fields

        private string _name;
        
        #endregion
        
        #region Public Proeprties

        public MuteTask MuteTask { get; set; }
        public int SliderIndex { get; set; }

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
                        if (SliderIndex >= ProfileHandler.SelectedProfile.Sliders.Count)
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
        public Guid UUID { get; set; }
        public ImageSource Image { get; set; } = Resource.MuteIcon.ToImageSource();
        public int Index { get; }

        #endregion
        
        #region Constructor

        public MuteFunction(int index, MuteTask muteTask, Guid uuid)
        {
            Index = index;
            MuteTask = muteTask;
            UUID = uuid;
        }

        public MuteFunction(int index, int sliderIndex, Guid uuid)
        {
            MuteTask = MuteTask.MuteSlider;
            Index = index;
            SliderIndex = sliderIndex;
            UUID = uuid;
        }

        static MuteFunction()
        {
            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;
            Initialize(false);
            
        }

        private static void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            Initialize(true);
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
                case MuteTask.MuteDefaultMic:
                    HandleMute(SessionHandler.DeviceEnumerator.DefaultInput);
                    break;
                case MuteTask.MuteDefaultSpeaker:
                    HandleMute(SessionHandler.DeviceEnumerator.DefaultOutput);
                    break;
                case MuteTask.MuteSlider:
                    HandleSliderMute(Index, SliderIndex);
                    break;
            }
        }

        public void ButtonKeyUp(int index)
        {
            
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Handle mute for specified device.
        /// </summary>
        /// <param name="device"></param>
        public static void HandleMute(MMDevice device)
        {
            var volumeEndpoint = device.AudioEndpointVolume;
            var mute = !volumeEndpoint.Mute;
            volumeEndpoint.Mute = mute;
            OverlayHandler.ShowMute(mute);
        }

        /// <summary>
        /// Handle current slider mute.
        /// </summary>
        /// <param name="sliderIndex"></param>
        public static void HandleSliderMute(int buttonIndex, int sliderIndex)
        {
            if (!_sliderMute.ContainsKey(sliderIndex))
                _sliderMute.Add(sliderIndex, false);
            var mute = !_sliderMute[sliderIndex];
            _sliderMute[sliderIndex] = mute;
            SessionHandler.SetMute(sliderIndex, mute, true);
            //DeviceNotifier.LightButton(unchecked((byte) buttonIndex), mute);
            OverlayHandler.ShowMute(mute);
        }

        #endregion
        
        #region Private Events & Methods
        
        private static void DeviceEnumeratorOnDeviceVolumeChanged(object sender, VolumeChangedArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { 
                var device = sender as MMDevice;

                var deviceId = device.ID;
                if (_lastMute.ContainsKey(deviceId))
                {
                    if (_lastMute[deviceId] == e.Mute)
                    {
                        SetLastMute(deviceId, e.Mute);
                        return;
                    }
                    SetLastMute(deviceId, e.Mute);
                }
                else
                    SetLastMute(deviceId, e.Mute);
                var defaultInputID = SessionHandler.DeviceEnumerator.DefaultInputID;
                var defaultOutputID = SessionHandler.DeviceEnumerator.DefaultOutputID;
                
                if (deviceId == defaultInputID)
                    for (var n = 0; n < _micMute.Count; n++)
                        DeviceNotifier.LightButton(unchecked((byte)_micMute[n].Index), e.Mute);
                else if (deviceId == defaultOutputID)
                    for (var n = 0; n < _speakerMute.Count; n++)
                        DeviceNotifier.LightButton(unchecked((byte)_speakerMute[n].Index), e.Mute);
            });
        }

        private static void ButtonHandlerOnFunctionCreated(object sender, FunctionArgs e)
        {
            if (e.Button is MuteFunction muteFunction)
                HandleFunctionAdd(muteFunction);
        }

        private static void ButtonHandlerOnFunctionRemoved(object sender, FunctionArgs e)
        {
            if(e.Button is MuteFunction muteFunction)
                HandleFunctionRemove(muteFunction);
        }

        /// <summary>
        /// Handle button function creation.
        /// </summary>
        /// <param name="muteFunction"></param>
        private static void HandleFunctionAdd(MuteFunction muteFunction)
        {
            switch (muteFunction.MuteTask)
            {
                case MuteTask.MuteDefaultMic:
                    _micMute.Add(muteFunction);
                    break;
                case MuteTask.MuteDefaultSpeaker:
                    _speakerMute.Add(muteFunction);
                    break;
                case MuteTask.MuteSlider:
                    var sliderIndex = muteFunction.SliderIndex;
                    var buttonIndex = muteFunction.Index;
                    if (SliderMute.ContainsKey(sliderIndex))
                    {
                        var buttonList = SliderMute[sliderIndex];
                        if (!buttonList.Contains(buttonIndex))
                            buttonList.Add(buttonIndex);
                        SliderMute[buttonIndex] = buttonList;
                    }
                    else
                    {
                        var buttonList = new List<int>();
                        buttonList.Add(buttonIndex);
                        SliderMute.Add(sliderIndex, buttonList);
                    }

                    
                    break;
            }
        }

        /// <summary>
        /// Handle button function removal.
        /// </summary>
        /// <param name="muteFunction"></param>
        private static void HandleFunctionRemove(MuteFunction muteFunction)
        {
            switch (muteFunction.MuteTask)
            {
                case MuteTask.MuteDefaultMic:
                    _micMute.Remove(muteFunction);
                    break;
                case MuteTask.MuteDefaultSpeaker:
                    _speakerMute.Remove(muteFunction);
                    break;
                case MuteTask.MuteSlider:
                    var sliderIndex = muteFunction.SliderIndex;
                    var buttonIndex = muteFunction.Index;
                    if(!SliderMute.ContainsKey(sliderIndex))
                        return;
                    var buttonList = SliderMute[sliderIndex];
                    if (buttonList.Count == 1 && buttonList.Contains(buttonIndex))
                        SliderMute.Remove(sliderIndex);
                    else if (buttonList.Count > 1)
                    {
                        if (buttonList.Contains(buttonIndex))
                            buttonList.Remove(buttonIndex);
                        SliderMute[sliderIndex] = buttonList;
                    }
                    break;
            }
        }

        /// <summary>
        /// Set last captured mute state for specified device.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mute"></param>
        private static void SetLastMute(string id, bool mute)
        {
            if (_lastMute.ContainsKey(id))
                _lastMute[id] = mute;
            else
                _lastMute.Add(id, mute);
        }

        /// <summary>
        /// Initialize global mute function handler.
        /// </summary>
        private static void Initialize(bool profileChange)
        {
            ButtonHandler.FunctionRemoved -= ButtonHandlerOnFunctionRemoved;
            ButtonHandler.FunctionCreated -= ButtonHandlerOnFunctionCreated;
            SessionHandler.DeviceEnumerator.DeviceVolumeChanged -= DeviceEnumeratorOnDeviceVolumeChanged;
            

            SliderMute.Clear();
            _micMute.Clear();
            _speakerMute.Clear();
            _lastMute.Clear();
            
            foreach (var button in ButtonHandler.Buttons)
                foreach (var func in button)
                    if (func is MuteFunction mediaFunc)
                        HandleFunctionAdd(mediaFunc);

            ButtonHandler.FunctionRemoved += ButtonHandlerOnFunctionRemoved;
            ButtonHandler.FunctionCreated += ButtonHandlerOnFunctionCreated;
            SessionHandler.DeviceEnumerator.DeviceVolumeChanged += DeviceEnumeratorOnDeviceVolumeChanged;
        }

        #endregion
    }

    public class MuteFunctionCreator : IButtonCreator
    {
        #region Implemented Methods
        
        public IButton CreateButton(int index, Dictionary<object, object> container, Guid uuid)
        {
            if(!container.ContainsKey(MuteFunction.MUTE_TASK_KEY))
                throw new NotImplementedException($"Container does not contains: {MuteFunction.MUTE_TASK_KEY} key");
            var muteTask = EnumUtils.Parse<MuteTask>(container[MuteFunction.MUTE_TASK_KEY].ToString());
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