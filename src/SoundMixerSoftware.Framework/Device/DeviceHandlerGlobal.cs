﻿using System;
using System.Collections.Generic;
using NLog;
using SoundMixerSoftware.Common.Communication;
using SoundMixerSoftware.Framework.Audio;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Overlay;
using SoundMixerSoftware.Framework.SliderConverter;
using SoundMixerSoftware.Interop.USBLib;

namespace SoundMixerSoftware.Framework.Device
{
    public static class DeviceHandlerGlobal
    {
        #region Current Class Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Static Fields

        private static Dictionary<string, DevicePair> _connectedDevices = new Dictionary<string, DevicePair>();

        #endregion
        
        #region Public Static Properties

        /// <summary>
        /// Global Device Handler.
        /// </summary>
        public static DeviceHandler Instance { get; }

        /// <summary>
        /// Currently connected devices.
        /// </summary>
        public static IReadOnlyDictionary<string, DevicePair> ConnectedDevice => _connectedDevices;
        
        public static OffsetManager ButtonOffsetManager { get; } = new OffsetManager();
        public static OffsetManager SliderOffsetManager { get; } = new OffsetManager();

        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when new device has connected.
        /// </summary>
        public static event EventHandler<DevicePair> DeviceConnected;
        /// <summary>
        /// Occurs when device has disconnected.
        /// </summary>
        public static event EventHandler<DevicePair> DeviceDisconnected;

        public static event EventHandler<SliderValueChanged> SliderValueChanged;
        public static event EventHandler<ButtonStateChanged> ButtonStateChanged;
        
        #endregion
        
        #region constructor

        static DeviceHandlerGlobal()
        {
            DeviceSettingsManager.Initialize();
            
            Instance = new DeviceHandler();
            Instance.DeviceConnected += DeviceHandlerOnDeviceConnected;
            Instance.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;
            Instance.DataReceived += DeviceHandlerOnDataReceived;

            foreach (var deviceSetting in DeviceSettingsManager.AllSettings.DeviceSettings)
            {
                var settings = deviceSetting.Value;
                var deviceId = deviceSetting.Key;
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

            ButtonOffsetManager.OffsetChanged += (sender, args) => OffsetChangedEvent(sender, args, (settings, offset) => settings.ButtonOffset = offset);
            SliderOffsetManager.OffsetChanged += (sender, args) => OffsetChangedEvent(sender, args, (settings, offset) => settings.SliderOffset = offset);
        }

        #endregion
        
        #region Methods

        private static void OffsetChangedEvent(object sender, OffsetChangedArgs e, Action<DeviceSettings, int> setOffset)
        {
            var deviceId = e.DeviceId;
            var settings = DeviceSettingsManager.GetSettings(deviceId);
            setOffset(settings, e.Offset);
            DeviceSettingsManager.SetSettings(deviceId, settings);
            DeviceSettingsManager.Save();
        }

        private static void DeviceHandlerOnDeviceConnected(object sender, DevicePair e)
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
                    var sliderStruct = (SliderStruct)e.Data;

                    var sliderIndex = GetIndex(SliderOffsetManager, sliderStruct.slider, deviceId);
                    var valueFloat = ConverterHandler.ConvertValue(sliderIndex, sliderStruct.value, deviceId);
                    if (float.IsNaN(valueFloat))
                        return;
                    SliderValueChanged?.Invoke(null, new SliderValueChanged(deviceId, sliderIndex, valueFloat));

                    break;

                case (byte) Command.BUTTON_COMMAND:
                    var buttonStruct = (ButtonStruct)e.Data;

                    var buttonIndex = GetIndex(ButtonOffsetManager, buttonStruct.button, deviceId);
                    ButtonStateChanged?.Invoke(null, new ButtonStateChanged(deviceId, buttonIndex, buttonStruct.state));

                    break;
            }
        }

        private static void RegisterTypes()
        {
            Instance.RegisterType((byte)Command.SLIDER_COMMAND, typeof(SliderStruct));
            Instance.RegisterType((byte)Command.BUTTON_COMMAND, typeof(ButtonStruct));
        }

        private static int GetIndex(OffsetManager offsetManager, int index, DeviceId deviceId)
        {
            return index + (byte)offsetManager.GetOrCreateOffset(deviceId);
        }
        
        #endregion
    }
}