using System;

namespace SoundMixerSoftware.Common.Communication.Serial
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