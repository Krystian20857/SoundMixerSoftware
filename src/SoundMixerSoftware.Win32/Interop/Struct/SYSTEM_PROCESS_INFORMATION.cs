using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable IdentifierTypo

namespace SoundMixerSoftware.Win32.Interop.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_PROCESS_INFORMATION
    {
        public int NextEntryOffset;
        public int NumberOfThreads;
        public LARGE_INTEGER WorkingSetPrivateSize;
        public uint HardFaultCount;
        public uint NumberOfThreadsHighWatermark;
        public ulong CycleTime;
        public long CreateTime;
        public long UserTime;
        public long KernelTime;
        public UNICODE_STRING ImageName;
        public int BasePriority;
        public uint UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
        public int HandleCount;
        public int SessionId;
        public IntPtr UniqueProcessKey;
        public IntPtr PeakVirtualSize;
        public IntPtr VirtualSize;
        public uint PageFaultCount;
        public IntPtr PeakWorkingSetSize;
        public IntPtr WorkingSetSize;
        public IntPtr QuotaPeakPagedPoolUsage;
        public IntPtr QuotaPagedPoolUsage;
        public IntPtr QuotaPeakNonPagedPoolUsage;
        public IntPtr QuotaNonPagedPoolUsage;
        public IntPtr PagefileUsage;
        public IntPtr PeakPagefileUsage;
        public IntPtr PrivatePageCount;
        public LARGE_INTEGER ReadOperationCount;
        public LARGE_INTEGER WriteOperationCount;
        public LARGE_INTEGER OtherOperationCount;
        public LARGE_INTEGER ReadTransferCount;
        public LARGE_INTEGER WriteTransferCount;
        public LARGE_INTEGER OtherTransferCount;
    }
}