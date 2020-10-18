using System;
using Caliburn.Micro;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Input;

namespace SoundMixerSoftware.Utils
{
    
    public static class ParserHelper
    {
        /// <summary>
        /// Configure parser for shortcuts.
        /// </summary>
        /// Code base on: https://github.com/Caliburn-Micro/Caliburn.Micro/blob/master/samples/scenarios/Scenario.KeyBinding/Bootstrapper.cs
        public static void ConfigureShortCuts()
        {
            var defaultCreateTrigger = Parser.CreateTrigger;
            Parser.CreateTrigger = (target, triggerText) =>
            {
                if (triggerText == null) return defaultCreateTrigger(target, null);
                
                var splits = TrimText(triggerText).Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                switch (splits[0])
                {
                    case "Shortcut":
                        var formatter = new KeyFormatter(splits[1]);
                        return new KeyTrigger(formatter.Key, formatter.Modifiers);
                }
                
                return defaultCreateTrigger(target, triggerText);
            };
        }

        /// <summary>
        /// Trim text to parse.
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static string TrimText(string inputText) => inputText.Replace(string.Empty, "[", "]");
    }
}