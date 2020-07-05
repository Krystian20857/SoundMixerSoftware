using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using GregsStack.InputSimulatorStandard.Native;

namespace SoundMixerSoftware.Common.Utils
{
    public static class KeyUtils
    {
        /// <summary>
        /// Convert wpf keyboard key implementation to win32 key implementation.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static VirtualKeyCode ConvertKey(Key key) => (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(key);

        /// <summary>
        /// Convert win32 keyboard key implementation to wpf key implementation.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Key ConvertKey(VirtualKeyCode key) => KeyInterop.KeyFromVirtualKey((int)key);

        /// <summary>
        /// Convert array of wpf keyboard key implementation to array win32 key implementation.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static VirtualKeyCode[] ConvertKeys(Key[] keys)
        {
            var keyArray = new VirtualKeyCode[keys.Length];
            for (var n = 0; n < keys.Length; n++)
                keyArray[n] = ConvertKey(keys[n]);
            return keyArray;
        }
        
        /// <summary>
        /// Convert array of win32 keyboard key implementation to array wpf key implementation.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static Key[] ConvertKeys(VirtualKeyCode[] keys)
        {
            var keyArray = new Key[keys.Length];
            for (var n = 0; n < keys.Length; n++)
                keyArray[n] = ConvertKey(keys[n]);
            return keyArray;
        }
        
    }
}