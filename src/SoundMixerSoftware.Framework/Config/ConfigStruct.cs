﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using SoundMixerSoftware.Common.Communication.Serial;
using SoundMixerSoftware.Common.Config;
using SoundMixerSoftware.Common.Utils;
using YamlDotNet.Serialization;

namespace SoundMixerSoftware.Framework.Config
{
    [Recursion]
    public class ConfigStruct : IConfigStruct<ConfigStruct>
    {
        #region Static Sample Config

        public static readonly ConfigStruct SampleConfigStruct = new ConfigStruct
        {
            Hardware = new HardwareSettings
            {
                
                SerialConfig = new SerialConfig
                {
                    BaudRate = 115200,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    Timeout = 30000,
                    DtsEnable = true,
                    RtsEnable = true
                },
                Terminator = 0xFF,
            },
            Application = new ApplicationSettings
            {
                ProfilesOrder = new List<Guid>(),
                SelectedProfile = Guid.Empty,
                HideOnStartup = true
            },
            Notification = new NotificationSettings
            {
                EnableNotifications = true,
                NotificationShowTime = TimeSpan.FromMilliseconds(7000),
            },
            Overlay = new OverlaySettings
            {
                EnableOverlay = true,
                OverlayFadeTime = TimeSpan.FromMilliseconds(2500)
            },
            Interop = new InteropSettings
            {
                WatcherWait = 5000,
            },
            Updater = new UpdateConfig
            {
                AutoUpdate = false
            }

        };
        
        #endregion

        #region Implemented Methods

        public ConfigStruct GetSampleConfig()
        {
            return SampleConfigStruct;
        }

        #endregion
        
        #region Base Values

        [Recursion]
        public ApplicationSettings Application { get; set; }
        [Recursion]
        public HardwareSettings Hardware { get; set; }
        [Recursion]
        public OverlaySettings Overlay { get; set; }
        [Recursion]
        public NotificationSettings Notification { get; set; }
        [Recursion]
        public InteropSettings Interop { get; set; }
        [Recursion]
        [YamlMember(Alias = "Updater")]
        public UpdateConfig Updater { get; set; }

        #endregion
        
        
        #region Public Methods
        
        public object Clone()
        {
            return MemberwiseClone();
        }
        
        #endregion
    }
    
    public class OverlaySettings
    {
        #region Base Types
        
        [YamlMember(Alias = "FadeTime")]
        public int? OverlayFadeTimeNullable { get; set; }
        [YamlMember(Alias = "EnableOverlay")]
        public bool? EnableOverlayNullable { get; set; }
        
        #endregion
        
        #region Non-null Types
        
        [YamlIgnore]
        public TimeSpan OverlayFadeTime
        {
            get => TimeSpan.FromMilliseconds(OverlayFadeTimeNullable ?? 0);
            set => OverlayFadeTimeNullable = (int)value.TotalMilliseconds;
        }
        [YamlIgnore]
        public bool EnableOverlay
        {
            get => EnableOverlayNullable ?? false;
            set => EnableOverlayNullable = value;
        }
        
        #endregion
    }
    
    public class NotificationSettings
    {
        #region Base Types
        
        [YamlMember(Alias = "EnableNotifications")]
        public bool? EnableNotificationsNullable { get; set; }
        
        [YamlMember(Alias = "NotificationShowTime")]
        public int? NotificationShowTimeNullable { get; set; }
        
        #endregion
        
        #region Non-null Types
        
        [YamlIgnore]
        public TimeSpan NotificationShowTime
        {
            get => TimeSpan.FromMilliseconds(NotificationShowTimeNullable ?? 0);
            set => NotificationShowTimeNullable = (int)value.TotalMilliseconds;
        }

        [YamlIgnore]
        public bool EnableNotifications
        {
            get => EnableNotificationsNullable ?? false;
            set => EnableNotificationsNullable = value;
        }

        #endregion
    }
    
    public class HardwareSettings
    {
        #region Base Types
        
        [YamlMember(Alias = "Terminator")]
        public byte? TerminatorNullable { get; set; }
        
        [Recursion]
        public SerialConfig SerialConfig { get; set; }

        #endregion
        
        #region Non-null Types
        
        [YamlIgnore]
        public byte Terminator
        {
            get => TerminatorNullable ?? 127;
            set => TerminatorNullable = value;
        }
        
        #endregion
    }

    public class ApplicationSettings
    {
        #region Base Types
        
        public Guid SelectedProfile { get; set; }
        public List<Guid> ProfilesOrder { get; set; }
        public string ThemeName { get; set; }
        [YamlMember(Alias = nameof(HideOnStartup))]
        public bool? HideOnStartupNullable { get; set; }
        [YamlMember(Alias = nameof(UseDarkTheme))]
        public bool? UseDarkThemeNullable { get; set; }

        #endregion
        
        #region Non-null Types
        
        [YamlIgnore]
        public bool HideOnStartup
        {
            get => HideOnStartupNullable ?? false;
            set => HideOnStartupNullable = value;
        }

        [YamlIgnore]
        public bool UseDarkTheme
        {
            get => UseDarkThemeNullable ?? false;
            set => UseDarkThemeNullable = value;
        }

        #endregion
    }

    public class DeviceSettings
    {
        public int SliderOffset { get; set; }
        public int ButtonOffset { get; set; }
    }

    public class InteropSettings
    {
        #region Base Type

        [YamlMember(Alias="WatcherWait")]
        public int? WatcherWaitNullable { get; set; }

        #endregion
        
        #region Non-null Types

        [YamlIgnore]
        public int WatcherWait
        {
            get => WatcherWaitNullable ?? 1;
            set => WatcherWaitNullable = value;
        }

        #endregion
    }

    public class UpdateConfig
    {
        #region BaseTypes
        
        [YamlMember(Alias = "AutoUpdate")]
        public bool? AutoUpdateNullable { get; set; }

        #endregion
        
        #region Non-null Types

        [YamlIgnore]
        public bool AutoUpdate
        {
            get => AutoUpdateNullable ?? false;
            set => AutoUpdateNullable = value;
        }

        #endregion
    }
}