using System;
using System.Collections.Generic;
using System.Windows;
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
        
        #endregion
        
        #region Static Fields

        private static IDictionary<string, bool> _lastMute =  new Dictionary<string, bool>();
        private static List<MuteFunction> _speakerMute = new List<MuteFunction>();
        private static List<MuteFunction> _micMute = new List<MuteFunction>();
        
        #endregion
        
        #region Private Fields

        private string _name;
        
        #endregion
        
        #region Public Proeprties

        public MuteTask MuteTask { get; set; }

        #endregion
        
        #region Implemented Properties
        
        public string Name
        {
            get
            {
                _name = EnumNameConverter.GetName(typeof(MuteTask), MuteTask.ToString());
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

        static MuteFunction()
        {
            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;
            Initialize();
            
        }

        private static void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            Initialize();
        }

        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(MUTE_TASK_KEY, MuteTask);
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
        private static void Initialize()
        {
            SessionHandler.DeviceEnumerator.DeviceVolumeChanged += DeviceEnumeratorOnDeviceVolumeChanged;

            _lastMute.Clear();
            foreach (var button in ButtonHandler.Buttons)
            foreach (var func in button)
                if (func is MuteFunction mediaFunc)
                    HandleFunctionAdd(mediaFunc);

            ButtonHandler.FunctionRemoved += ButtonHandlerOnFunctionRemoved;
            ButtonHandler.FunctionCreated += ButtonHandlerOnFunctionCreated;
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
            var muteTask = container[MuteFunction.MUTE_TASK_KEY].ToString();
            var muteTaskEnum = EnumUtils.Parse<MuteTask>(muteTask);
            return new MuteFunction(index, muteTaskEnum, uuid);
        }
        
        #endregion
    }

    public enum MuteTask
    {
        [ValueName("Mute/Unmute Default Speaker")]MuteDefaultSpeaker,
        [ValueName("Mute/Unmute Default Microphone")]MuteDefaultMic
    }
}