namespace SoundMixerSoftware.Common.Extension
{
    /// <summary>
    /// Contains Extension to string class.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Replace multiple chains in string.
        /// </summary>
        /// <param name="inputText">Base string chain.</param>
        /// <param name="newValue">New char chain value.</param>
        /// <param name="oldValues">Strings chains to replace.</param>
        /// <returns>Output string.</returns>
        public static string Replace(this string inputText, string newValue, params string[] oldValues)
        {
            for (var n = 0; n < oldValues.Length; n++)
                inputText = inputText.Replace(oldValues[n], newValue);
            return inputText;
        }
        
        /// <summary>
        /// Replace multiple characters in string.
        /// </summary>
        /// <param name="inputText">Base string chain.</param>
        /// <param name="newValue">New char value.</param>
        /// <param name="oldValues">Characters to replace.</param>
        /// <returns>Output string.</returns>
        public static string Replace(this string inputText, char newValue, params char[] oldValues)
        {
            for (var n = 0; n < oldValues.Length; n++)
                inputText = inputText.Replace(oldValues[n], newValue);
            return inputText;
        }
    }
}