using System;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib
{
    
    public class DeviceStateChangedArgs : EventArgs
    {
        /// <summary>
        /// Current device state
        /// </summary>
        public DeviceState DeviceState { get; set; }

        public DeviceStateChangedArgs(DeviceState deviceState)
        {
            DeviceState = deviceState;
        }
    }

    public class DefaultDeviceChangedArgs : EventArgs
    {
        /// <summary>
        /// Current device data flow.
        /// </summary>
        public DataFlow DataFlow { get; set; }
        /// <summary>
        /// Current device role.
        /// </summary>
        public Role Role { get; set; }
        
        public DefaultDeviceChangedArgs(DataFlow dataFlow, Role role)
        {
            DataFlow = dataFlow;
            Role = role;
        }
    }

    public class PropertyChangedArgs : EventArgs
    {
        /// <summary>
        /// Current property key.
        /// </summary>
        public PropertyKey PropertyKey { get; set; }

        public PropertyChangedArgs(PropertyKey propertyKey)
        {
            PropertyKey = propertyKey;
        }
    }

    public class VolumeChangedArgs : EventArgs
    {
        /// <summary>
        /// Volumne 
        /// </summary>
        public float Volume { get; set; }
        /// <summary>
        /// Mute
        /// </summary>
        public bool Mute { get; set; }
        /// <summary>
        /// True when volume was set by current device enumerator.
        /// </summary>
        public bool IsFromApp { get; set; }

        public VolumeChangedArgs(float volume, bool mute, bool isFromApp)
        {
            Volume = volume;
            Mute = mute;
            IsFromApp = isFromApp;
        }
    }
    
    public class ChannelVolumeChangedArgs : EventArgs{
        public uint ChannelCount { get; set; }
        public IntPtr NewVolumes { get; set; }
        public uint ChannelIndex { get; set; }

        public ChannelVolumeChangedArgs(uint channelCount, IntPtr newVolumes, uint channelIndex)
        {
            ChannelCount = channelCount;
            NewVolumes = newVolumes;
            ChannelIndex = channelIndex;
        }
    }
}