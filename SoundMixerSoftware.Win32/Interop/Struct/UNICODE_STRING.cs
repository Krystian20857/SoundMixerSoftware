﻿using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Win32.Interop.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct UNICODE_STRING
    {
        public uint Length;
        public uint MaximumLength;
        public IntPtr Buffer;
    }
}