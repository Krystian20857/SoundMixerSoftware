﻿using Caliburn.Micro;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Device;
using SoundMixerSoftware.Interop.USBLib;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of Usb Manager Window.
    /// </summary>
    public class UsbManagerViewModel : Screen
    {
        #region Private Fields
        
        private BindableCollection<USBID> _USBIds = new BindableCollection<USBID>();
        private USBID _selectedItem;
        
        #endregion
        
        #region Public Properties

        public static UsbManagerViewModel Instance = IoC.Get<UsbManagerViewModel>();

        /// <summary>
        /// Available USB ids(vid, pid)
        /// </summary>
        public BindableCollection<USBID> USBIds
        {
            get => _USBIds;
            set
            {
                _USBIds = value;
                NotifyOfPropertyChange(nameof(USBIds));
            }
        }

        /// <summary>
        /// Current added Vendor id.
        /// </summary>
        public uint Vid { get; set; }
        /// <summary>
        /// Current added Product id.
        /// </summary>
        public uint Pid { get; set; }

        /// <summary>
        /// Currently Selected USBID.
        /// </summary>
        public USBID SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (value == null) return;
                Vid = value.Vid;
                NotifyOfPropertyChange(nameof(Vid));
                Pid = value.Pid;
                NotifyOfPropertyChange(nameof(Pid));
            }
        }

        #endregion
        
        #region Private Properties
        
        
        #endregion
        
        #region Constructor

        public UsbManagerViewModel()
        {
            foreach (var usbid in DeviceSettingsManager.AllSettings.UsbIDs)
                USBIds.Add(usbid);
        }
        
        #endregion

        #region Private Events

        /// <summary>
        /// Occurs when Save Button has clicked.
        /// </summary>
        public void Save()
        {
            var usbIds = DeviceSettingsManager.AllSettings.UsbIDs;
            var usbId = new USBID { Pid = Pid, Vid = Vid };
            if (usbIds.Contains(usbId)) return;
            USBIds.Add(usbId);
            usbIds.Add(usbId);
            DeviceSettingsManager.Save();
        }

        /// <summary>
        /// Occurs when Remove button has clicked.
        /// </summary>
        public void Remove()
        {
            DeviceSettingsManager.AllSettings.UsbIDs.Remove(SelectedItem);
            USBIds.Remove(SelectedItem);
            DeviceSettingsManager.Save();
        }

        /// <summary>
        /// Occurs when Cancel button has clicked.
        /// </summary>
        public void Close()
        {
            TryCloseAsync();
        }

        #endregion
    }
}