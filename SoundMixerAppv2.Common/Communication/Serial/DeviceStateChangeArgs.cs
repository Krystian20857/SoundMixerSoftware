using System;

namespace SoundMixerAppv2.Common.Communication.Serial
{
    public class DeviceStateChangeArgs : EventArgs
    {
        public string COMPort { get; set; }

        public DeviceStateChangeArgs(string comPort)
        {
            COMPort = comPort;
        }
    }
}