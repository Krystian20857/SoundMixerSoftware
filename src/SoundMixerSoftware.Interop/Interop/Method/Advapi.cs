using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SoundMixerSoftware.Interop.Method
{
    public static class Advapi
    {
        [DllImport("advapi32.dll", SetLastError=true)]
        public static extern int RegCloseKey(IntPtr hKey);
        
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        public static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, out uint lpType, StringBuilder lpData, ref uint lpcbData);
    }
}