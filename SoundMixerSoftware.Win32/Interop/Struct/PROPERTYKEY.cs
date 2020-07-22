using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Win32.Interop.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PROPERTYKEY
    {
        public Guid fmtid;
        public UIntPtr pid;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var key = (PROPERTYKEY) obj;
            return this.fmtid == key.fmtid && this.pid == key.pid;
        }

        public override int GetHashCode()
        {
            return fmtid.GetHashCode() + pid.GetHashCode();
        }
    }
}