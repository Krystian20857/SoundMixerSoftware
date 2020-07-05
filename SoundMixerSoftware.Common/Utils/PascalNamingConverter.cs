namespace SoundMixerSoftware.Common.Utils
{
    public static class PascalNamingConverter
    {

        /// <summary>
        /// Apply spacing to string using Pascal naming convention. 
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static string ApplySpacing(string input)
        {
            for (var n = 0; n < input.Length; n++)
            {
                if (n + 1 == input.Length)
                    break;
                if (char.IsLower(input[n]) && char.IsUpper(input[n + 1]))
                    input = input.Insert(n + 1, " ");
                else if (char.IsUpper(input[n]) && char.IsUpper(input[n + 1]))
                    input = input.Insert(n + 1, "-");
            }
            return input;
        }
        
    }
}