﻿using System.Text;
using GregsStack.InputSimulatorStandard.Native;
using SoundMixerSoftware.Win32.Win32;

namespace SoundMixerSoftware.Win32.Wrapper
{
    public static class KeyWrapper
    {
        /// <summary>
        /// Get chars represents keycode.
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool GetChars(VirtualKeyCode keyCode, out string result)
        {
            var keyBuilder = new StringBuilder(256);
            var keyboardState = new byte[256];
            var foundChars = NativeMethods.ToUnicode((uint)keyCode, 0, keyboardState, keyBuilder, 256, 0);
            result = keyBuilder.ToString();
            return foundChars > 0;
        }
        
    }
}