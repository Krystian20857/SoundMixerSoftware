using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.Overlay;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Win32.Wrapper;
using VolumeChangedArgs = SoundMixerSoftware.Common.AudioLib.VolumeChangedArgs;

namespace SoundMixerSoftware.Helpers.Buttons
{
    public static class ButtonHandler
    {
        #region Private Fields

        private static bool _lastMuteInput;
        private static bool _lastMuteOutput;
        
        #endregion
        
        #region Public Properties

        #endregion
        
        #region Constructor

        static ButtonHandler()
        {

        }
        
        #endregion
        
        #region Private Events

        private static void DeviceEnumeratorOnDeviceVolumeChanged(object sender, VolumeChangedArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var device = sender as MMDevice;
                var defaultInputID = SessionHandler.DeviceEnumerator.DefaultInput.ID;
                var defaultOutputID = SessionHandler.DeviceEnumerator.DefaultOutput.ID;
                var mute = e.Mute;
                if (device.ID == defaultInputID)
                {
                    if (mute != _lastMuteInput)
                    {
                        DisplayMute(false, mute);
                        _lastMuteInput = mute;
                    }
                }
                else if (device.ID == defaultOutputID)
                {
                    if (mute != _lastMuteOutput)
                    {
                        DisplayMute(true, mute);
                        _lastMuteOutput = mute;
                    }
                }
            });
        }

        #endregion
        
        #region Public Static Methods

        /// <summary>
        /// Handle function of specified button.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public static bool HandleButton(ButtonFunction function)
        {
            switch (function)
            {
                case ButtonFunction.MuteInput:
                    HandleMuteInput();
                    break;
                case ButtonFunction.MuteOutput:
                    HandleMuteOutput();
                    break;
                case ButtonFunction.PrevTrack:
                    MediaControl.PrevTrack();
                    break;
                case ButtonFunction.NextTrack:
                    MediaControl.NextTrack();
                    break;
                case ButtonFunction.PausePlay:
                    MediaControl.PauseResume();
                    break;
                default:
                    return false;
            }
            
            return true;
        }

        public static void DisplayMute(bool isOutput, bool state)
        {
            foreach (var device in DeviceHandlerGlobal.ConnectedDevice)
            {
                var buttons = ProfileHandler.SelectedProfile.Buttons;
                for (var n = 0; n < buttons.Count; n++)
                {
                    var button = buttons[n];
                    var structure = new LedStruct
                    {
                        command = 0x01,
                        state = (byte) (state ? 0x01 : 0x00),
                        led = (byte) n,
                    };
                    if (isOutput)
                    {
                        if (button.Function == ButtonFunction.MuteOutput)
                            DeviceHandlerGlobal.DeviceHandler.SendData(device.Key, structure);
                    }
                    else if (button.Function == ButtonFunction.MuteInput)
                        DeviceHandlerGlobal.DeviceHandler.SendData(device.Key, structure);

                }
            }
        }

        #endregion
        
        #region Public Static Methods

        public static void Initialize()
        {
            SessionHandler.DeviceEnumerator.DeviceVolumeChanged += DeviceEnumeratorOnDeviceVolumeChanged;
            _lastMuteOutput = SessionHandler.DeviceEnumerator.DefaultOutput.AudioEndpointVolume.Mute;
            _lastMuteInput = SessionHandler.DeviceEnumerator.DefaultInput.AudioEndpointVolume.Mute;
        }
        
        #endregion

        #region Private Static Methods

        internal static void HandleMuteInput()
        {
            var defaultInput = SessionHandler.DeviceEnumerator.DefaultInput;
            var audioEndpoint = defaultInput.AudioEndpointVolume;
            audioEndpoint.Mute = !audioEndpoint.Mute;
            ShowOverlay(audioEndpoint.Mute);
        }

        internal static void HandleMuteOutput()
        {
            var defaultInput = SessionHandler.DeviceEnumerator.DefaultOutput;
            var audioEndpoint = defaultInput.AudioEndpointVolume;
            audioEndpoint.Mute = !audioEndpoint.Mute;
            ShowOverlay(audioEndpoint.Mute);
        }

        private static void ShowOverlay(bool mute)
        {
            OverlayHandler.MuteOverlay.IsMuted = mute;
            if(!OverlayHandler.MuteOverlay.IsVisible)
                OverlayHandler.MuteOverlay.ShowWindow();
        }
        
        #endregion
    }
}