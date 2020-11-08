using System;
using System.Collections.Generic;
using NLog;
using SoundMixerSoftware.Common.Communication;
using SoundMixerSoftware.Framework.Audio;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Overlay;
using SoundMixerSoftware.Framework.SliderConverter;
using SoundMixerSoftware.Win32.USBLib;

namespace SoundMixerSoftware.Framework.Device
{
    public static class DeviceHandlerGlobal
    {
        #region Current Class Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Static Fields

        private static Dictionary<string, DeviceConnectedEventArgs> _connectedDevices = new Dictionary<string, DeviceConnectedEventArgs>();

        #endregion
        
        #region Public Static Properties

        /// <summary>
        /// Global Device Handler.
        /// </summary>
        public static DeviceHandler Instance { get; }

        /// <summary>
        /// Currently connected devices.
        /// </summary>
        public static IReadOnlyDictionary<string, DeviceConnectedEventArgs> ConnectedDevice => _connectedDevices;
        
        public static OffsetManager ButtonOffsetManager { get; } = new OffsetManager();
        public static OffsetManager SliderOffsetManager { get; } = new OffsetManager();

        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when new device has connected.
        /// </summary>
        public static event EventHandler<DeviceConnectedEventArgs> DeviceConnected;
        /// <summary>
        /// Occurs when device has disconnected.
        /// </summary>
        public static event EventHandler<DeviceConnectedEventArgs> DeviceDisconnected;

        public static event EventHandler<SliderValueChanged> SliderValueChanged;
        public static event EventHandler<ButtonStateChanged> ButtonStateChanged;
        
        #endregion
        
        #region constructor

        static DeviceHandlerGlobal()
        {
            Instance = new DeviceHandler();
            Instance.DeviceConnected += DeviceHandlerOnDeviceConnected;
            Instance.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;
            Instance.DataReceived += DeviceHandlerOnDataReceived;
            
            ButtonOffsetManager.OffsetChanged += ButtonOffsetManagerOnOffsetChanged;
            SliderOffsetManager.OffsetChanged += SliderOffsetManagerOnOffsetChanged;

            foreach (var deviceSetting in ConfigHandler.ConfigStruct.Hardware.DeviceSettings)
            {
                if (!DeviceId.TryParse(deviceSetting.Key, out var deviceId))
                    continue;
                var settings = deviceSetting.Value;
                ButtonOffsetManager.SetOffset(deviceId, settings.ButtonOffset, false);
                SliderOffsetManager.SetOffset(deviceId, settings.SliderOffset, false);
            }

            RegisterTypes();

            SliderValueChanged += (sender, args) =>
            {
                var value = (int) args.Value;
                var index = args.Index;
                
                SessionHandler.SetVolume(index, value, false);
                if(SessionHandler.HasActiveSession(index))
                    OverlayHandler.ShowVolume(value);
            };

            ButtonStateChanged += (sender, args) =>
            {
                var state = args.State;
                var index = args.Index;
                
                if (state == ButtonState.DOWN)
                    ButtonHandler.HandleKeyDown(index);
                else if (state == ButtonState.UP)
                    ButtonHandler.HandleKeyUp(index);
            };
        }

        #endregion
        
        #region Events
        
        private static void SliderOffsetManagerOnOffsetChanged(object sender, OffsetChangedArgs e)
        {
            var deviceId = e.DeviceId;
            var settings = DeviceSettingsManager.GetSettings(deviceId);
            settings.SliderOffset = e.Offset;
            DeviceSettingsManager.SetSettings(deviceId, settings);
            ConfigHandler.SaveConfig();
        }

        private static void ButtonOffsetManagerOnOffsetChanged(object sender, OffsetChangedArgs e)
        {
            var deviceId = e.DeviceId;
            var settings = DeviceSettingsManager.GetSettings(deviceId);
            settings.ButtonOffset = e.Offset;
            DeviceSettingsManager.SetSettings(deviceId, settings);
            ConfigHandler.SaveConfig();
        }

        private static void DeviceHandlerOnDeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Device.COMPort) || _connectedDevices.ContainsKey(e.Device.COMPort))
                return;
            DeviceConnected?.Invoke(null, e);
            _connectedDevices.Add(e.Device.COMPort, e);
        }

        private static void DeviceHandlerOnDeviceDisconnected(object sender, DeviceStateArgs e)
        {
            var comPort = e.DeviceProperties.COMPort;
            if(string.IsNullOrEmpty(comPort))
                return;
            if (_connectedDevices.TryGetValue(comPort, out var device))
            {
                DeviceDisconnected?.Invoke(null, device);
                _connectedDevices.Remove(comPort);
            }
            else
                Logger.Warn($"Device: {e.DeviceProperties.COMPort} not present.");
        }

        private static void DeviceHandlerOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!(e.Arguments is DeviceId deviceId))
                return;
            if (DeviceId.IsEmpty(deviceId))
                return;
            switch (e.Command)
            {
                case (byte) Command.SLIDER_COMMAND:
                    SliderStruct sliderStruct = e.Data;

                    var sliderIndex = GetSliderIndex(sliderStruct.slider, deviceId);
                    var valueFloat = ConverterHandler.ConvertValue(sliderIndex, sliderStruct.value, deviceId);
                    if (float.IsNaN(valueFloat))
                        return;
                    SliderValueChanged?.Invoke(null, new SliderValueChanged(deviceId, sliderIndex, valueFloat));

                    break;

                case (byte) Command.BUTTON_COMMAND:
                    ButtonStruct buttonStruct = e.Data;

                    var buttonIndex = GetButtonIndex(buttonStruct.button, deviceId);
                    ButtonStateChanged?.Invoke(null, new ButtonStateChanged(deviceId, buttonIndex, buttonStruct.state));

                    break;
            }
        }

        #endregion
        
        #region Static methods

        private static void RegisterTypes()
        {
            Instance.RegisterType((byte)Command.SLIDER_COMMAND, typeof(SliderStruct));
            Instance.RegisterType((byte)Command.BUTTON_COMMAND, typeof(ButtonStruct));
        }

        private static int GetSliderIndex(int index, DeviceId deviceId)
        {
            
            var sliderIndex = index + (byte)SliderOffsetManager.GetOrCreateOffset(deviceId);
            var sliderCount = SessionHandler.Sessions.Count;
            if (sliderIndex >= sliderCount)
            {
                Logger.Warn("Slider receive index mismatch.");
                return sliderCount - 1;
            }

            return sliderIndex;
        }
        
        private static int GetButtonIndex(int index, DeviceId deviceId)
        {
            var buttonIndex = index + (byte)ButtonOffsetManager.GetOrCreateOffset(deviceId);
            var buttonCount = ButtonHandler.Buttons.Count;
            if (buttonIndex >= buttonCount)
            {
                Logger.Warn("Button receive index mismatch.");
                return buttonCount - 1;
            }

            return buttonIndex;
        }
        
        #endregion
    }
}