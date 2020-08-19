using Caliburn.Micro;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class DeviceSettingsViewModel : Screen
    {
        #region Private Fields
        
        private DeviceModel _device = new DeviceModel();

        #endregion
        
        #region Public Properties

        public static DeviceSettingsViewModel Instance => IoC.Get<DeviceSettingsViewModel>();

        public DeviceModel Device
        {
            get => _device;
            set
            {
                _device = value;
                
                SliderOffset = DeviceHandlerGlobal.SliderOffsetManager.GetOrCreateOffset(value.UUID);
                ButtonOffset = DeviceHandlerGlobal.ButtonOffsetManager.GetOrCreateOffset(value.UUID);
                MaxSliderOffset = ProfileHandler.SelectedProfile.SliderCount - Device.Sliders;
                MaxButtonOffset = ProfileHandler.SelectedProfile.ButtonCount - Device.Buttons;
                
                NotifyOfPropertyChange(nameof(SliderOffset));
                NotifyOfPropertyChange(nameof(ButtonOffset));
            }
        }

        public int SliderOffset { get; set; }
        public int ButtonOffset { get; set; }

        
        public int MaxSliderOffset { get; set; }
        public int MaxButtonOffset { get; set; }

        #endregion

        #region Constructor

        public DeviceSettingsViewModel() { }

        #endregion
        
        #region Private Events

        public void AddClick()
        {
            var deviceId = _device.UUID;
            var deviceSettings = DeviceSettingsManager.GetSettings(deviceId);
            deviceSettings.ButtonOffset = ButtonOffset;
            deviceSettings.SliderOffset = SliderOffset;
            DeviceHandlerGlobal.SliderOffsetManager.SetOffset(deviceId, SliderOffset);
            DeviceHandlerGlobal.ButtonOffsetManager.SetOffset(deviceId, ButtonOffset);
            DeviceSettingsManager.SetSettings(deviceId, deviceSettings);
            ConfigHandler.SaveConfig();
            TryCloseAsync();
        }

        public void CancelClick()
        {
            TryCloseAsync();
        }
        
        #endregion
    }

    
}