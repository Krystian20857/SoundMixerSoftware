using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using SoundMixerSoftware.Common.Communication.Serial;
using SoundMixerSoftware.Common.Config;
using SoundMixerSoftware.Win32.USBLib;
using YamlDotNet.Serialization;

namespace SoundMixerSoftware.Helpers.Config
{
    public class ConfigStruct : IConfigStruct<ConfigStruct>
    {
        #region Static Sample Config

        public static ConfigStruct SampleConfigStruct = new ConfigStruct()
        {
            UsbIDs = new List<USBID>()
            {
                new USBID{ Vid = 0x468F, Pid= 0x895D}
            },
            SerialConfig = new SerialConfig()
            {
                BaudRate = 115200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Timeout = 30000
            },
            Terminator = 0xFF,
        };
        
        #endregion

        #region Implemented Properties
        
        [YamlIgnore]
        public ConfigStruct SampleConfig => SampleConfigStruct;
        
        #endregion
        
        #region Public Properties

        public List<USBID> UsbIDs { get; set; }
        public SerialConfig SerialConfig { get; set; }
        public byte Terminator { get; set; }

        #endregion
        
        #region Public Methods
        
        public ConfigStruct Copy()
        {
            return (ConfigStruct)MemberwiseClone();
        }
        
        #endregion
    }
}