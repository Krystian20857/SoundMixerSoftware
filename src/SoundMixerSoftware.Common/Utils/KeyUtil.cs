using System.Text;
using System.Windows.Input;
using GregsStack.InputSimulatorStandard.Native;
using SoundMixerSoftware.Interop.Wrapper;

namespace SoundMixerSoftware.Common.Utils
{
    public static class KeyUtil
    {
        /// <summary>
        /// Convert wpf keyboard key implementation to win32 key implementation.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static VirtualKeyCode ConvertKey(Key key) => (VirtualKeyCode) KeyInterop.VirtualKeyFromKey(key);

        /// <summary>
        /// Convert win32 keyboard key implementation to wpf key implementation.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Key ConvertKey(VirtualKeyCode key) => KeyInterop.KeyFromVirtualKey((int) key);

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

        /// <summary>
        /// Apply proper naming to key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeyName(Key key)
        {
            switch (key)
            {
                case Key.Enter:
                    return "Enter";
                case Key.CapsLock:
                    return "CapsLock";
                case Key.Escape:
                    return "Escape";
                case Key.PrintScreen:
                    return "Print Screen";
                case Key.Back:
                    return "Backspace";
                case Key.Space:
                    return "Space";
                default:
                    return KeyWrapper.GetChars(ConvertKey(key), out var result) ? result.ToUpper() : PascalNamingConverter.ApplySpacing(key.ToString());
            }
        }

        /// <summary>
        /// Format key with modifiers.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        public static string FormatKeys(Key key, Key[] modifiers)
        {
            var keyBuilder = new StringBuilder();
            for (var n = 0; n < modifiers.Length; n++)
            {
                keyBuilder.Append(GetKeyName(modifiers[n]));
                if (n != modifiers.Length - 1)
                    keyBuilder.Append(" + ");
            }

            if (key != default)
            {
                if (modifiers.Length > 0)
                    keyBuilder.Append(" + ");
                keyBuilder.Append(GetKeyName(key));
            }

            return keyBuilder.ToString();
        }

        /// <summary>
        /// Check if key is modifer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsModifierKey(Key key)
        {
            switch (key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LWin:
                case Key.RWin:
                case Key.LeftAlt:
                case Key.RightAlt:
                    return true;

                default:
                    return false;
            }
        }

    }
}