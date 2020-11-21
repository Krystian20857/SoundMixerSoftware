using System;

namespace SoundMixerSoftware.Interop.USBLib
{
    // ReSharper disable once InconsistentNaming
    public class HwId : IEquatable<HwId>, ICloneable
    {
        #region Public Properties
        
        public uint Vid { get; set; }
        public uint Pid { get; set; }
        
        #endregion
        
        #region Overriden Methods
        public override int GetHashCode()
        {
            return 215134142;
        }

        public object Clone()
        {
            return new HwId { Vid = Vid, Pid = Pid};
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (obj.GetType() != GetType()) return false;
            return Equals((HwId) obj);
        }

        public override string ToString()
        {
            return $"VID: {Vid}, PID: {Pid}";
        }

        public bool Equals(HwId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Vid == other.Vid && Pid == other.Pid;
        }

        public static bool operator ==(HwId usbid1, HwId usbid2)
        {
            return Equals(usbid1, usbid2);
        }

        public static bool operator !=(HwId usbid1, HwId usbid2)
        {
            return !(usbid1 == usbid2);
        }

        #endregion
    }
}