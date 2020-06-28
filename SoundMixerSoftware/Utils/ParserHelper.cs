using System;
using System.Linq;
using Caliburn.Micro;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Input;

namespace SoundMixerSoftware.Utils
{
    public static class ParserHelper
    {
        public static void ConfigureShortCuts()
        {
            var defaultCreateTrigger = Parser.CreateTrigger;
            Parser.CreateTrigger = (target, triggerText) =>
            {
                if (string.IsNullOrEmpty(triggerText)) return defaultCreateTrigger(target, string.Empty);
                
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

        public static string TrimText(string inputText) => inputText.Replace(string.Empty, "[", "]");
    }
}