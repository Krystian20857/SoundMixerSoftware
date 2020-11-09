using System;

// ReSharper disable UnassignedField.Global
// ReSharper disable InconsistentNaming

namespace SoundMixerSoftware.Interop.Struct
{
    public struct PROCESS_EXTENDED_BASIC_INFORMATION {
        public UIntPtr Size;
        public PROCESS_BASIC_INFORMATION BasicInfo;
        public uint Flags;
    }
}