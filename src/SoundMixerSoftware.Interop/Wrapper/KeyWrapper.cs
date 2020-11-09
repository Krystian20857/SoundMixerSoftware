using System.Text;
using GregsStack.InputSimulatorStandard.Native;
using SoundMixerSoftware.Interop.Method;

namespace SoundMixerSoftware.Interop.Wrapper
{
    public static class KeyWrapper
    {
        /// <summary>
        /// Get chars represents keycode.
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GetChars(VirtualKeyCode keyCode, out string result)
        {
            var keyBuilder = new StringBuilder(256);
            var keyboardState = new byte[256];
            var foundChars = User32.ToUnicode((uint)keyCode, 0, keyboardState, keyBuilder, 256, 0);
            result = keyBuilder.ToString();
            return foundChars > 0;
        }
        
    }
}