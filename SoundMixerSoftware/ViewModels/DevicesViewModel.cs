using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class DevicesViewModel : ITabModel
    {
        #region Private Properties
        
        private BindableCollection<DeviceModel> _devices = new BindableCollection<DeviceModel>();
        
        #endregion
        
        #region Public Properties

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
            
            Devices.Add(new DeviceModel()
            {
                Name="My Sound Mixer",
                Buttons = "5",
                ComPort= "Com5",
                Pid = "0x0453",
                Vid = "0x6325",
                Sliders="6"
            });
        }
        
        #endregion
    }
}