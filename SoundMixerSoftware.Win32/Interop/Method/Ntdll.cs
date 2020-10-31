using System;
using System.Runtime.InteropServices;
using SoundMixerSoftware.Win32.Interop.Enum;
using SoundMixerSoftware.Win32.Interop.Struct;

namespace SoundMixerSoftware.Win32.Interop.Method
{
    public static class Ntdll
    {
        [DllImport("ntdll.dll", PreserveSig = false, SetLastError = true)]
        public static extern void NtQueryInformationProcess(IntPtr processHandle, PROCESSINFOCLASS processInformationClass, out PROCESS_EXTENDED_BASIC_INFORMATION processInformation, uint processInformationLength, out uint returnLength);
        
        [DllImport("ntdll.dll", PreserveSig = true, EntryPoint = "NtQuerySystemInformation")]
        public static extern NtStatus NtQuerySystemInformationInitial(SYSTEM_INFORMATION_CLASS infoClass, IntPtr info, int size, out int length);
        
        [DllImport("ntdll.dll", PreserveSig = true)]
        public static extern NtStatus NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS systemInformationClass, IntPtr systemInformation, int systemInformationLength, out uint returnLength);

    }
}