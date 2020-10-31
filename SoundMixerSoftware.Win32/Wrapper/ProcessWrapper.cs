using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SoundMixerSoftware.Win32.Interop.Enum;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Interop.Struct;

namespace SoundMixerSoftware.Win32.Wrapper
{
    public static class ProcessWrapper
    {
        /// <summary>
        /// Get Process file name by standard WIN32 method
        /// </summary>
        /// <param name="pid">process id</param>
        /// <param name="buffer">buffer size default: 260</param>
        /// <returns></returns>
        public static string GetFileName(int pid, int buffer = 260) //260 -> max windows path length
        {
            var handle = Kernel32.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, pid);
            if (handle == IntPtr.Zero)
                return string.Empty;
            var nameBuilder = new StringBuilder(buffer);
            var bufferLength = nameBuilder.Capacity + 1;
            return Kernel32.QueryFullProcessImageName(handle, 0, nameBuilder, ref bufferLength) ? nameBuilder.ToString() : string.Empty;
        }
        
        /// <summary>
        /// Check if process id exists. Useful with zombie audio sessions war :).
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns></returns>
        public static bool IsAlive(int pid)
        {
            var phandle = IntPtr.Zero;
            try
            {
                phandle = Kernel32.OpenProcess(ProcessAccessFlags.Synchronize, false, pid);
                if (phandle == IntPtr.Zero)
                    return false;
                return Kernel32.WaitForSingleObject(phandle, 0) != 0;
            }
            finally
            {
                Kernel32.CloseHandle(phandle);
            }
        }
        
        /// <summary>
        /// Get running processes using win32 api.
        /// </summary>
        /// <returns></returns>
        public static uint[] EnumProcesses()
        {
            var buffer = new uint[1024];
            var bufferSize = (uint)buffer.Length * sizeof(uint);
            var status = Psapi.EnumProcesses(buffer, bufferSize, out var bytesCopied);

            if (!status || bytesCopied == 0)
                return default;

            var copiedIds = (int)bytesCopied >> 2;
            Array.Resize(ref buffer, copiedIds);
            return buffer;
        }

        /// <summary>
        /// Get child process ids of process.
        /// </summary>
        /// <param name="pid">parent process id.</param>
        /// <param name="includeParent">include self process</param>
        /// <returns></returns>
        public static IEnumerable<uint> GetChildProcesses(uint pid, bool includeParent = true)
        {
            if (includeParent)
                yield return pid;
            var processes = EnumProcesses();
            for (var n = 0; n < processes.Length; n++)
            {
                var process = processes[n];
                var processHandle = IntPtr.Zero;
                try
                {
                    processHandle = Kernel32.OpenProcess(ProcessAccessFlags.QueryInformation, false, (int) process);
                    if(processHandle == IntPtr.Zero)
                        continue;
                    Ntdll.NtQueryInformationProcess(processHandle, PROCESSINFOCLASS.ProcessBasicInformation, out var information, (uint)Marshal.SizeOf<PROCESS_EXTENDED_BASIC_INFORMATION>(), out var _);
                    var parentId = information.BasicInfo.InheritedFromUniqueProcessId;
                    if (parentId.ToUInt32() == pid)
                    {
                        yield return process;
                    }
                }
                finally
                {
                    Kernel32.CloseHandle(processHandle);
                }
            }
        }

        /// <summary>
        /// Gets parent of specified process via interop.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static uint GetParentProcess(uint pid)
        {
            var processHandle = IntPtr.Zero;
            try
            {
                processHandle = Kernel32.OpenProcess(ProcessAccessFlags.QueryInformation, false, (int) pid);
                Ntdll.NtQueryInformationProcess(processHandle, PROCESSINFOCLASS.ProcessBasicInformation, out var information, (uint)Marshal.SizeOf<PROCESS_EXTENDED_BASIC_INFORMATION>(), out var _);
                if (processHandle == IntPtr.Zero)
                    return 0;
                return information.BasicInfo.InheritedFromUniqueProcessId.ToUInt32();
            }
            finally
            {
                Kernel32.CloseHandle(processHandle);
            }
        }
    }
}