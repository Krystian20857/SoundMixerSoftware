using System;
using System.Runtime.InteropServices;
using SoundMixerSoftware.Win32.Interop.Enum;
using SoundMixerSoftware.Win32.Interop.Struct;

namespace SoundMixerSoftware.Win32.Interop.Method
{
    public static class Ntdll
    {
        [DllImport("ntdll.dll", PreserveSig = false, SetLastError = true)]
        public static extern void NtQueryInformationProcess(IntPtr ProcessHandle, PROCESSINFOCLASS ProcessInformationClass, out PROCESS_EXTENDED_BASIC_INFORMATION ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

    }
}