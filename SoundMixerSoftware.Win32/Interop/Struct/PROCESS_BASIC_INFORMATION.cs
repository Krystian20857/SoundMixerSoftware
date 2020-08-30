using System;
using SoundMixerSoftware.Win32.Interop.Enum;

namespace SoundMixerSoftware.Win32.Interop.Struct
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