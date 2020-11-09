using System;
using System.Runtime.InteropServices;
using SoundMixerSoftware.Interop.Enum;

namespace SoundMixerSoftware.Interop.Interface
{
    [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItem
    {
        void BindToHandle([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, ref Guid riid, out IntPtr ppv);
        void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
        void GetDisplayName([In] SIGDN sigdn, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
        void GetAttribute([In] uint mask, out uint psfgaoAttribute);
        void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
    }
}