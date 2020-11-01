using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Common.Utils.EnumUtils;

namespace SoundMixerSoftware.Framework.Buttons.Functions
{
    public class KeystrokeFunction : IButtonFunction
    {
        #region Constatn

        public const string KEY = "keystroke_func";

        public const string KEYSTROKE_MODE_KEY = "mode";
        public const string TEXT_KEY = "text";
        public const string KEYSTROKE_KEY = "keystroke";
        public const string MODIFIERS_KEY = "modifiers";
        
        #endregion
        
        #region Private Fields
        
        private IKeyboardSimulator _simulator = new InputSimulator().Keyboard;
        private string _name;

        #endregion
        
        #region Public Properties

        public VirtualKeyCode Keystroke { get; set; }
        public VirtualKeyCode[] ModifierKeys { get; set; }

        public string Text { get; set; }

        public KeystrokeMode KeystrokeMode { get; set; }

        #endregion

        #region Implemented Proeprties

        public string Name
        {
            get
            {
                _name = this.ToString();
                return _name;
            }
            set => _name = value;
        }
        public string Key { get; } = KEY;
        public int Index { get; set; }
        public Guid UUID { get; set; }
        public ImageSource Image { get; set; } = Resource.KeyboardIcon.ToImageSource();
        
        #endregion
        
        #region Constructor

        public KeystrokeFunction(VirtualKeyCode keystroke, VirtualKeyCode[] modifiers, Guid uuid)
        {
            KeystrokeMode = KeystrokeMode.KeyPress;
            Keystroke = keystroke;
            ModifierKeys = modifiers;
            UUID = uuid;
        }
        
        public KeystrokeFunction(Key keystroke, Key[] modifiers, Guid uuid)
        {
            KeystrokeMode = KeystrokeMode.KeyPress;
            Keystroke = KeyUtil.ConvertKey(keystroke);
            ModifierKeys = KeyUtil.ConvertKeys(modifiers);
            UUID = uuid;
        }

        public KeystrokeFunction(string text, Guid uuid)
        {
            KeystrokeMode = KeystrokeMode.TextMode;
            Text = text;
            UUID = uuid;
        }
        
        public KeystrokeFunction(int index, VirtualKeyCode keystroke, VirtualKeyCode[] modifiers, Guid uuid) : this(keystroke, modifiers, uuid)
        {
            Index = index;
        }
        
        public KeystrokeFunction(int index, Key keystroke, Key[] modifiers, Guid uuid) : this(keystroke, modifiers, uuid)
        {
            Index = index;
        }

        public KeystrokeFunction(int index, string text, Guid uuid) : this(text, uuid)
        {
            Index = index;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(KEYSTROKE_MODE_KEY, KeystrokeMode);
            switch (KeystrokeMode)
            {
                case KeystrokeMode.KeyPress:
                    result.Add(KEYSTROKE_KEY, Keystroke);
                    result.Add(MODIFIERS_KEY, ModifierKeys);
                    break;
                case KeystrokeMode.TextMode:
                    result.Add(TEXT_KEY, Text);
                    break;
            }

            return result;
        }

        public void ButtonKeyDown(int index)
        {
            switch (KeystrokeMode)
            {
                case KeystrokeMode.KeyPress:
                    for (var n = 0; n < ModifierKeys.Length; n++)
                        _simulator.KeyDown(ModifierKeys[n]);
                    _simulator.KeyDown(Keystroke);
                    _simulator.Sleep(1);
                    for (var n = 0; n < ModifierKeys.Length; n++)
                        _simulator.KeyUp(ModifierKeys[n]);
                    _simulator.KeyUp(Keystroke);
                    break;
                case KeystrokeMode.TextMode:
                    _simulator.TextEntry(Text);
                    break;
            }
        }

        public void ButtonKeyUp(int index)
        {
            
        }
        
        #endregion
        
        #region Public Static Methods


        
        #endregion
        
        #region Overriden Methods

        public override string ToString()
        {
            switch (KeystrokeMode)
            {
                case KeystrokeMode.KeyPress:
                    return $"Key: {KeyUtil.FormatKeys(KeyUtil.ConvertKey(Keystroke), KeyUtil.ConvertKeys(ModifierKeys))}";
                case KeystrokeMode.TextMode:
                    return $"Text: {Text}";
                default:
                    return base.ToString();
            }
        }

        #endregion
    }
    
    public class KeystrokeFunctionCreator : IButtonCreator {
        public IButtonFunction CreateButton(int index, Dictionary<object, object> container, Guid uuid)
        {
            if (!container.ContainsKey(KeystrokeFunction.KEYSTROKE_MODE_KEY))
                return null;
            var mode = EnumUtil.Parse<KeystrokeMode>(container[KeystrokeFunction.KEYSTROKE_MODE_KEY]);
            switch (mode)
            {
                case KeystrokeMode.KeyPress:
                    var keystroke = EnumUtil.Parse<VirtualKeyCode>(container[KeystrokeFunction.KEYSTROKE_KEY]);
                    var modifiers = EnumUtil.Parse<VirtualKeyCode>((List<object>)container[KeystrokeFunction.MODIFIERS_KEY]);
                    return new KeystrokeFunction(index, keystroke, modifiers.ToArray(), uuid);
                case KeystrokeMode.TextMode:
                    var text = container[KeystrokeFunction.TEXT_KEY];
                    return new KeystrokeFunction(index, text.ToString(), uuid);
                default:
                    return null;
            }
        }
    }

    public enum KeystrokeMode
    {
        [ValueName("Enter Text")]TextMode,
        [ValueName("Press Key Combination")]KeyPress
    }
}