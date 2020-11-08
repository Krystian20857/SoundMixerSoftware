using System;

namespace SoundMixerSoftware.Common.Communication.Serial
{
    public class SerialDataReceivedArgs : EventArgs
    {
        /// <summary>
        /// Represents received data.
        /// </summary>
        public byte[] Data { get; set; }

        public string COMPort { get; set; }

        public SerialDataReceivedArgs(byte[] data, string comPort)
        {
            Data = data;
            COMPort = comPort;
        }
    }
}