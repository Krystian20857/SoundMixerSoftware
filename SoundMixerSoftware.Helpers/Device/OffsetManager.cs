using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Helpers.Device
{
    public class OffsetManager: IDisposable
    {
        #region Private Fields

        #endregion
        
        #region Public Properties

        public Dictionary<DeviceId, int> Offset { get; } = new Dictionary<DeviceId, int>();

        #endregion
        
        #region Events

        public event EventHandler<OffsetChangedArgs> OffsetChanged;
        
        #endregion
        
        #region Constructor

        public OffsetManager()
        {
            
        }
        
        #endregion
        
        #region Public Methods

        public int GetOrCreateOffset(DeviceId deviceId, int defaultValue = 0)
        {
            if (!Offset.ContainsKey(deviceId))
            {
                SetOffset(deviceId, defaultValue);
                return defaultValue;
            }

            return Offset[deviceId];
        }

        public void SetOffset(DeviceId deviceId, int offset, bool fireEvent = true)
        {
            if (Offset.ContainsKey(deviceId))
                Offset[deviceId] = offset;
            else
                Offset.Add(deviceId, offset);
            if(fireEvent)
                OffsetChanged?.Invoke(this, new OffsetChangedArgs(deviceId, offset));
        }
        #endregion
        
        #region Dispose

        public void Dispose()
        {
            Offset?.Clear();
        }
        
        #endregion
    }

    public class OffsetChangedArgs : EventArgs
    {
        public DeviceId DeviceId { get; set; }
        public int Offset { get; set; }

        public OffsetChangedArgs(DeviceId deviceId, int offset)
        {
            DeviceId = deviceId;
            Offset = offset;
        }
    }
}