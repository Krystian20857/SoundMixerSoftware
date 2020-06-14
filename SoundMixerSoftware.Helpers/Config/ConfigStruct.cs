using System;
using System.Collections.Generic;
using System.IO.Ports;
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
            ProfilesOrder = new List<Guid>(),
            EnableNotifications = true,
            OverlayFadeTimeNullable = 2500,
            EnableOverlay = true,
            NotificationShowTime = 7000,
            ThemeName = "Purple"
        };
        
        #endregion

        #region Implemented Properties
        
        [YamlIgnore]
        public ConfigStruct SampleConfig => SampleConfigStruct;
        
        #endregion
        
        #region Base Values
        public List<USBID> UsbIDs { get; set; }
        public SerialConfig SerialConfig { get; set; }
        public Guid SelectedProfile { get; set; }
        public List<Guid> ProfilesOrder { get; set; }
        #endregion
        
        #region Nullable value-types
        [YamlMember(Alias = "Terminator")]
        public byte? TerminatorNullable { get; set; }
        [YamlMember(Alias = "EnableNotifications")]
        public bool? EnableNotificationsNullable { get; set; }
        [YamlMember(Alias = "FadeTime")]
        public int? OverlayFadeTimeNullable { get; set; }
        [YamlMember(Alias = "EnableOverlay")]
        public bool? EnableOverlayNullable { get; set; }
        [YamlMember(Alias = "NotificationShowTime")]
        public int? NotificationShowTimeNullable { get; set; }

        public string ThemeName { get; set; }

        #endregion
        
        #region Non-null value-types

        [YamlIgnore]
        public bool EnableNotifications
        {
            get => EnableNotificationsNullable ?? false;
            set => EnableNotificationsNullable = value;
        }
        [YamlIgnore]
        public int OverlayFadeTime
        {
            get => OverlayFadeTimeNullable ?? 0;
            set => OverlayFadeTimeNullable = value;
        }
        [YamlIgnore]
        public bool EnableOverlay
        {
            get => EnableOverlayNullable ?? false;
            set => EnableOverlayNullable = value;
        }
        [YamlIgnore]
        public byte Terminator
        {
            get => TerminatorNullable ?? 127;
            set => TerminatorNullable = value;
        }

        [YamlIgnore]
        public int NotificationShowTime
        {
            get => NotificationShowTimeNullable ?? 0;
            set => NotificationShowTimeNullable = value;
        }

        #endregion
        
        #region Public Methods
        
        public ConfigStruct Copy()
        {
            return (ConfigStruct)MemberwiseClone();
        }
        
        #endregion
    }
}