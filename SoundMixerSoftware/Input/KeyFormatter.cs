using System;
using System.Windows.Input;

namespace SoundMixerSoftware.Input
{
    public class KeyFormatter
    {
        #region Private Fields
        
        
        #endregion
        
        #region Public Properties

        public string Shortcut { get; set; }
        public ModifierKeys Modifiers { get; protected set; }
        public Key Key { get; protected set; }

        #endregion
        
        #region Constructor

        public KeyFormatter(string shortcut) : this()
        {
            Shortcut = shortcut;
            Format();
        }
        
        public KeyFormatter(){}
        
        #endregion
        
        #region Public Methods

        public void Format()
        {
            var splits = Shortcut.Split(new[] {'+'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var key in splits)
            {
                if (Enum.TryParse<Key>(key, out var resultKey))
                    Key = resultKey;
                if (Enum.TryParse<ModifierKeys>(key, out var resultModifier))
                    Modifiers = resultModifier;
            }
        }

        #endregion
    }
}