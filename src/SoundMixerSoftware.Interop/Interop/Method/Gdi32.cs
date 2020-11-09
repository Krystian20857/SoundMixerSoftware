using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Interop.Method
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}