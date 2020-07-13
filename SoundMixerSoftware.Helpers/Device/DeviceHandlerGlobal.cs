using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using NLog;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.NotifyWrapper;
using SoundMixerSoftware.Helpers.Overlay;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Helpers.SliderConverter;
using SoundMixerSoftware.Win32.USBLib;
using DataReceivedEventArgs = SoundMixerSoftware.Common.Communication.DataReceivedEventArgs;

namespace SoundMixerSoftware.Helpers.Device
{
    public static class DeviceHandlerGlobal
    {
        #region Current Class Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Static Fields

        private static Dictionary<string, DeviceConnectedEventArgs> _connectedDevices = new Dictionary<string, DeviceConnectedEventArgs>();
        private static DeviceNotification _deviceNotification = new DeviceNotification();

        #endregion
        
        #region Public Static Properties

        /// <summary>
        /// Global Device Handler.
        /// </summary>
        public static DeviceHandler DeviceHandler { get; }

        /// <summary>
        /// Currently connected devices.
        /// </summary>
        public static IReadOnlyDictionary<string, DeviceConnectedEventArgs> ConnectedDevice => _connectedDevices;
        
        public static OffsetManager ButtonOffsetManager { get; } = new OffsetManager();
        public static OffsetManager SliderOffsetManager { get; } = new OffsetManager();

        #endregion
        
        #region constructor

        static DeviceHandlerGlobal()
        {
            DeviceHandler = new DeviceHandler();
            DeviceHandler.DeviceConnected += DeviceHandlerOnDeviceConnected;
            DeviceHandler.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;
            DeviceHandler.DataReceived += DeviceHandlerOnDataReceived;
            
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
        }

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

        #endregion
        
        #region Events

        private static void DeviceHandlerOnDeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Device.COMPort) || _connectedDevices.ContainsKey(e.Device.COMPort))
                return;
            _connectedDevices.Add(e.Device.COMPort, e);
            if (!e.DetectedOnStartup && ConfigHandler.ConfigStruct.Notification.EnableNotifications)
            {
                _deviceNotification.SetValue(DeviceNotification.EVENT_ARGS_KEY, e);
                _deviceNotification.SetValue(DeviceNotification.DEVICE_STATE_KEY, DeviceNotificationState.Connected);
                _deviceNotification.Show();
            }
        }

        private static void DeviceHandlerOnDeviceDisconnected(object sender, DeviceStateArgs e)
        {
            var comPort = e.DeviceProperties.COMPort;
            if (_connectedDevices.TryGetValue(comPort, out var device))
            {
                _connectedDevices.Remove(comPort);
                if (ConfigHandler.ConfigStruct.Notification.EnableNotifications)
                {
                    _deviceNotification.SetValue(DeviceNotification.EVENT_ARGS_KEY, device);
                    _deviceNotification.SetValue(DeviceNotification.DEVICE_STATE_KEY, DeviceNotificationState.Disconnected);
                    _deviceNotification.Show();
                }
            }
            else
                Logger.Warn($"Device: {e.DeviceProperties.COMPort} not present.");
        }

        private static void DeviceHandlerOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            var deviceId = e.Arguments as DeviceId;
            if (deviceId == null || DeviceId.IsEmpty(deviceId))
                return;
            switch (e.Command)
            {
                case 0x01:
                    SliderStruct sliderStruct = e.Data;
                    var sliderIndex = sliderStruct.slider + (byte)SliderOffsetManager.GetOrCreateOffset(deviceId);
                    if (sliderIndex >= SessionHandler.Sliders.Count)
                    {
                        Logger.Warn("Slider receive index mismatch.");
                        return;
                    }

                    var value = (int)ConverterHandler.ConvertValue(sliderIndex, sliderStruct.value, deviceId);
                    SessionHandler.SetVolume(sliderIndex, value / 100.0F, false);
                    if(SessionHandler.Sliders[sliderIndex].Count > 0)
                        OverlayHandler.ShowVolume(value);
                    break;
                case 0x02:
                    ButtonStruct buttonStruct = e.Data;
                    var buttonIndex = buttonStruct.button + (byte)ButtonOffsetManager.GetOrCreateOffset(deviceId);
                    var profile = ProfileHandler.SelectedProfile;
                    if (buttonIndex >= profile.ButtonCount)
                    {
                        Logger.Warn("Button receive index mismatch.");
                        return;
                    }
                    
                    if (buttonStruct.state == 0x00) 
                        ButtonHandler.HandleKeyDown(buttonIndex);
                    else if(buttonStruct.state == 0x01)
                        ButtonHandler.HandleKeyUp(buttonIndex);
                    break;
            }
        }
        
        #endregion
        
        #region Static methods

        private static void RegisterTypes()
        {
            DeviceHandler.RegisterType(0x01, typeof(SliderStruct));
            DeviceHandler.RegisterType(0x02, typeof(ButtonStruct));
        }
        
        #endregion
    }
}