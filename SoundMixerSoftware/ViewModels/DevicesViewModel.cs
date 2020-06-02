using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Device;
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

        #endregion
        
        #region Public Properties

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

        #endregion
        
        #region Constructor

        public DevicesViewModel()
        {
            Name = "Devices";
            Icon = PackIconKind.CodeBraces;
            
            DeviceHandlerGlobal.DeviceHandler.DeviceConnected += DeviceHandlerOnDeviceConnected;
            DeviceHandlerGlobal.DeviceHandler.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;
        }

        /// <summary>
        /// Occurs when new device has succesfully cnnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceHandlerOnDeviceDisconnected(object sender, DeviceStateArgs e)
        {
            //This type of action need to be handled in main thread.
            Execute.OnUIThread(() =>
            {
                var _device = Devices.First(x => x.ComPort.Equals(e.DeviceProperties.COMPort, StringComparison.InvariantCultureIgnoreCase));
                if(Devices.Contains(_device))
                Devices.Remove(_device);
            });
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
                Devices.Add(DeviceModel.CreateModel(e.Device, e.DeviceResponse));
            });
        }

        #endregion
        
        #region Private Events

        /// <summary>
        /// Occurs when Device Manager button has clicked. 
        /// </summary>
        public void ManagerClick()
        {
            var usbManager = new UsbManagerViewModel();
            _windowManager.ShowDialog(usbManager);
        }

        /// <summary>
        /// Occurs when check button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void CheckClick(object sender)
        {
            var devicemodel = sender as DeviceModel;
            DeviceNotifier.TestLights(devicemodel.ComPort);
        }
        #endregion
    }
}