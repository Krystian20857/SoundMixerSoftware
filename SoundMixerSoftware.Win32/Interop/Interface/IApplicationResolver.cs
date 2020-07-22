using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Win32.Interop.Interface
{
    [ComImport]
    [Guid("660B90C8-73A9-4B58-8CAE-355B7F55341B")]
    public class ApplicationResolver
    {
        
    }
    
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("DE25675A-72DE-44b4-9373-05170450C140")]
    public interface IApplicationResolver
    {
        void unused1();
        void unused2();
        void unused3();
        void GetAppIDForProcess(uint pid, [MarshalAs(UnmanagedType.LPWStr)]out string appId, out IntPtr unused1, out IntPtr unused2, out IntPtr unused3);
    }
}