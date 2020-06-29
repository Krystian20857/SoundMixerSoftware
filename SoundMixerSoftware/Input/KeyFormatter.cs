using System;
using System.Windows.Input;

namespace SoundMixerSoftware.Input
{
    public class KeyFormatter
    {
        #region Private Fields
        
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Sets and gets shortcut to parse.
        /// </summary>
        public string Shortcut { get; set; }
        /// <summary>
        /// Gets parsed modifiers.
        /// </summary>
        public ModifierKeys Modifiers { get; protected set; }
        /// <summary>
        /// Get parsed key shortcut.
        /// </summary>
        public Key Key { get; protected set; }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Create instance of KeyFormatter and parse shortcut.
        /// </summary>
        /// <param name="shortcut"></param>
        public KeyFormatter(string shortcut) : this()
        {
            Shortcut = shortcut;
            Format();
        }
        
        public KeyFormatter(){}
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Manually format shortcut.
        /// </summary>
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