using System;
using SoundMixerSoftware.Interop.Enum;

// ReSharper disable InconsistentNaming

namespace SoundMixerSoftware.Interop.Struct
{
    public struct PROCESS_BASIC_INFORMATION
    {
        public NtStatus ExitStatus;
        public IntPtr PebBaseAddress;
        public UIntPtr AffinityMask;
        public int BasePriority;
        public UIntPtr UniqueProcessId;
        public UIntPtr InheritedFromUniqueProcessId;
    }
}