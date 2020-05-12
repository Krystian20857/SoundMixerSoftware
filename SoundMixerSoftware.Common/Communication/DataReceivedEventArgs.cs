using System;

namespace SoundMixerSoftware.Common.Communication
{
    public class DataReceivedEventArgs : EventArgs
    {
        #region Public Properties
        /// <summary>
        /// Represents byte command assigned to type.
        /// </summary>
        public byte Command { get; set; }
        /// <summary>
        /// Received converted data.
        /// </summary>
        public dynamic Data { get; set; }
        
        #endregion

        #region Constructor
        
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        public DataReceivedEventArgs(byte command, dynamic data)
        {
            Command = command;
            Data = data;
        }
        
        public DataReceivedEventArgs(){}
        
        #endregion
    }
}