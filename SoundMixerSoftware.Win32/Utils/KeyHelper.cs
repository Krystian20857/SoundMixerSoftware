using System;
using System.Runtime.InteropServices;
using SoundMixerAppv2.Win32.Win32;

namespace SoundMixerAppv2.Win32.Utils
{
    public static class KeyHelper
    {
        /// <summary>
        /// Creates key.
        /// </summary>
        /// <param name="vkey">Virtual key.</param>
        /// <param name="scan">Scan key.</param>
        /// <param name="dwFlags">Extra info ptr.</param>
        /// <returns>Input.</returns>
        public static NativeStructs.INPUT CreateKey(NativeEnums.VirtualKeyShort vKey)
        {
            return new NativeStructs.INPUT
            {
                type = NativeEnums.INPUT_TYPE.INPUT_KEYBOARD,
                inputUnion = new NativeStructs.InputUnion
                {
                    keyboardinput = new NativeStructs.KEYBDINPUT
                    {
                        wVk = vKey,
                        wScan = (NativeEnums.ScanCodeShort)(NativeMethods.MapVirtualKey((uint)vKey,0) & 0xFE),
                        dwExtraInfo = UIntPtr.Zero,
                    }
                }
            };
        }

        /// <summary>
        /// Simulate key press.
        /// </summary>
        /// <param name="input"></param>
        public static void KeyPress(NativeStructs.INPUT input)
        {
            input.inputUnion.keyboardinput.dwFlags = 0;
            NativeMethods.SendInput(1, new[] {input}, Marshal.SizeOf(input));
        }

        /// <summary>
        /// Simulate key release.
        /// </summary>
        /// <param name="input"></param>
        public static void KeyRelease(NativeStructs.INPUT input)
        {
            input.inputUnion.keyboardinput.dwFlags = NativeEnums.KEYEVENTF.KEYUP;
            NativeMethods.SendInput(1, new[] {input}, Marshal.SizeOf(input));        
        }

        /// <summary>
        /// Press and release key.
        /// </summary>
        /// <param name="input"></param>
        public static void KeyClick(NativeStructs.INPUT input)
        {
            input.inputUnion.keyboardinput.dwFlags = NativeEnums.KEYEVENTF.EXTENDEDKEY;
            NativeMethods.SendInput(1, new[] {input}, Marshal.SizeOf(input));
        }
    }
}