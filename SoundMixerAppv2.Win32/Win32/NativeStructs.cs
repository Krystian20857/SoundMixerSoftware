using System;
using System.Runtime.InteropServices;

namespace SoundMixerAppv2.Win32.Win32
{
    public class NativeStructs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal NativeEnums.INPUT_TYPE type;
            internal InputUnion inputUnion;
            internal static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mouseinput;
            [FieldOffset(0)]
            internal KEYBDINPUT keyboardinput;
            [FieldOffset(0)]
            internal HARDWAREINPUT hardwareinput;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal NativeEnums.MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal NativeEnums.VirtualKeyShort wVk;
            internal NativeEnums.ScanCodeShort wScan;
            internal NativeEnums.KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

    }
}