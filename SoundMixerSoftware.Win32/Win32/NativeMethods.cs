using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SoundMixerSoftware.Win32.Win32
{
    public static class NativeMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex, out IntPtr phiconLarge,
            out IntPtr phiconSmall, uint nIcons);

        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] NativeStructs.INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterDeviceNotification(IntPtr Handle);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(IntPtr guid, [MarshalAs(UnmanagedType.LPTStr)] string Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref NativeStructs.SP_DEVINFO_DATA DeviceInfoData);
        
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref NativeStructs.SP_DEVINFO_DATA deviceInfoData, uint property, out int propertyRegDataType, IntPtr propertyBuffer, uint propertyBufferSize, out int requiredSize);

        [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref NativeStructs.SP_DEVINFO_DATA deviceInfoData, int scope, int hwProfile, int parameterRegistryValueKind, int samDesired);
        
        [DllImport("advapi32.dll", SetLastError=true)]
        public static extern int RegCloseKey(IntPtr hKey);
        
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        public static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, out uint lpType, System.Text.StringBuilder lpData, ref uint lpcbData);
        
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
        
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool QueryFullProcessImageName([In]IntPtr hProcess, [In]int dwFlags, [Out]StringBuilder lpExeName, ref int lpdwSize);
    }
}