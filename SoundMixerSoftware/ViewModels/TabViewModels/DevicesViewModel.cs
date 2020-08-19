using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.NotifyWrapper;
using SoundMixerSoftware.Helpers.Overlay;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Win32.USBLib;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of Devices tab in main window.
    /// </summary>
    public class DevicesViewModel : ITabModel
    {
        #region Private Properties
        
        /// <summary>
        /// Use in window handling.
        /// </summary>
        private IWindowManager _windowManager =  new WindowManager();
        
        private BindableCollection<DeviceModel> _devices = new BindableCollection<DeviceModel>();
        private INotification<object> _deviceNotification = new DeviceNotification(); 

        #endregion
        
        #region Public Properties
        
        public static DevicesViewModel Instance => IoC.Get<DevicesViewModel>();

        /// <summary>
        /// Collection of device models.
        /// </summary>
        public BindableCollection<DeviceModel> Devices
        {
            get => _devices;
            set
            {
                _devices = value;
            }
        }

        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }
        public Guid Uuid { get; set; } = new Guid("3845ECE9-16B9-41BD-87A4-DD9C19EFCE10");

        #endregion
        
        #region Constructor

        public DevicesViewModel()
        {
            Name = "Devices";
            Icon = PackIconKind.CodeBraces;

            RuntimeHelpers.RunClassConstructor(typeof(DeviceHandlerGlobal).TypeHandle);
            
            DeviceHandlerGlobal.DeviceConnected += DeviceHandlerOnDeviceConnected;
            DeviceHandlerGlobal.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;

            _deviceNotification.Clicked += () => Bootstrapper.Instance.BringToFront();
        }

        /// <summary>
        /// Occurs when new device has successfully connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceHandlerOnDeviceDisconnected(object sender, DeviceConnectedEventArgs e)
        {
            //This type of action need to be handled in main thread.
            var comPort = e.Device.COMPort;
            Execute.OnUIThread(() =>
            {
                var device = Devices.FirstOrDefault(x => x.ComPort.Equals(comPort, StringComparison.InvariantCultureIgnoreCase));
                var deviceIndex = Devices.IndexOf(device);
                if(deviceIndex >= 0)
                    Devices.RemoveAt(deviceIndex);
            });
            if (ConfigHandler.ConfigStruct.Notification.EnableNotifications)
            {
                _deviceNotification.SetValue(DeviceNotification.EVENT_ARGS_KEY, e);
                _deviceNotification.SetValue(DeviceNotification.DEVICE_STATE_KEY, DeviceNotificationState.Disconnected);
                _deviceNotification.Show();
            }
        }

        /// <summary>
        /// Occurs when device has disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceHandlerOnDeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            //This type of action need to be handled in main thread.
            Execute.OnUIThread(() =>
            {
                Devices.Add(DeviceModel.CreateModel(e));
            });
            if (!e.DetectedOnStartup && ConfigHandler.ConfigStruct.Notification.EnableNotifications)
            {
                _deviceNotification.SetValue(DeviceNotification.EVENT_ARGS_KEY, e);
                _deviceNotification.SetValue(DeviceNotification.DEVICE_STATE_KEY, DeviceNotificationState.Connected);
                _deviceNotification.Show();
            }
        }

        #endregion
        
        #region Private Events

        /// <summary>
        /// Occurs when Device Manager button has clicked. 
        /// </summary>
        public void ManagerClick()
        {
            var usbManager = UsbManagerViewModel.Instance;
            _windowManager.ShowDialogAsync(usbManager);
        }

        /// <summary>
        /// Occurs when check button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void CheckClick(object sender)
        {
            var devicemodel = sender as DeviceModel;
            DeviceNotifier.TestLights(devicemodel.ComPort);
            if(ConfigHandler.ConfigStruct.Overlay.EnableOverlay)
                OverlayHandler.ShowText("Check lights on your device.");
        }

        public void SettingsClick(object sender)
        {
            var deviceModel = sender as DeviceModel;
            var settingsManager = DeviceSettingsViewModel.Instance;
            settingsManager.Device = deviceModel;
            _windowManager.ShowDialogAsync(settingsManager);
        }
        
        #endregion
    }
}