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
        public object Data { get; set; }
        /// <summary>
        /// Additional arguments.
        /// </summary>
        public object Arguments { get; set; }

        #endregion

        #region Constructor
        
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        public DataReceivedEventArgs(byte command, object data, object arguments)
        {
            Command = command;
            Data = data;
            Arguments = arguments;
        }
        
        public DataReceivedEventArgs(){}
        
        #endregion
    }
}