using Caliburn.Micro;
using SoundMixerSoftware.Common.Config;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Win32.USBLib;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of Usb Manager Window.
    /// </summary>
    public class UsbManagerViewModel : Screen
    {
        #region Private Fields
        
        private BindableCollection<USBID> _USBIds = new BindableCollection<USBID>();
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Available USB ids(vid, pid)
        /// </summary>
        public BindableCollection<USBID> USBIds
        {
            get => _USBIds;
            set
            {
                _USBIds = value;
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
        public USBID SelectedItem { get; set; }

        #endregion
        
        #region Private Properties
        
        
        #endregion
        
        #region Constructor

        public UsbManagerViewModel()
        {
            foreach (var usbid in ConfigHandler.ConfigStruct.UsbIDs)
                USBIds.Add(usbid);
        }
        
        #endregion

        #region Private Events

        /// <summary>
        /// Occurs when Save Button has clicked.
        /// </summary>
        public void Save()
        {
            var usbId = new USBID
            {
                Pid = Pid,
                Vid = Vid
            };
            USBIds.Add(usbId);
            ConfigHandler.ConfigStruct.UsbIDs.Add(usbId);
            ConfigHandler.SaveConfig();
        }

        /// <summary>
        /// Occurs when Remove button has clicked.
        /// </summary>
        public void Remove()
        {
            ConfigHandler.ConfigStruct.UsbIDs.Remove(SelectedItem);
            USBIds.Remove(SelectedItem);
            ConfigHandler.SaveConfig();
        }

        /// <summary>
        /// Occurs when Cancel button has clicked.
        /// </summary>
        public void Cancel()
        {
            TryCloseAsync();
        }

        #endregion
    }
}