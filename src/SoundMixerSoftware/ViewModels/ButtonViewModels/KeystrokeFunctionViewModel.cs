﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Buttons.Functions;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Utils;

namespace SoundMixerSoftware.ViewModels
{
    public class KeystrokeFunctionViewModel : IButtonAddModel, INotifyPropertyChanged
    {
        #region Private Fields
        
        private EnumDisplayModel<KeystrokeMode> _selectedFunction;
        
        private bool _textVisibility;
        private bool _keystrokeVisibility;

        private string _keystrokeText = string.Empty;
        
        #endregion
        
        #region Public Properties

        public BindableCollection<EnumDisplayModel<KeystrokeMode>> Functions { get; set; } = new BindableCollection<EnumDisplayModel<KeystrokeMode>>();

        public EnumDisplayModel<KeystrokeMode> SelectedFunction
        {
            get => _selectedFunction;
            set
            {
                _selectedFunction = value;
                SetVisibilityFor(value.EnumValue);
            }
        }

        public bool TextVisibility
        {
            get => _textVisibility;
            set
            {
                _textVisibility = value;
                OnPropertyChanged(nameof(TextVisibility));
            }
        }

        public bool KeystrokeVisibility
        {
            get => _keystrokeVisibility;
            set
            {
                _keystrokeVisibility = value;
                OnPropertyChanged(nameof(KeystrokeVisibility));
            }
        }


        public string KeystrokeText
        {
            get => _keystrokeText;
            set
            {
                _keystrokeText = value;
                OnPropertyChanged(nameof(KeystrokeText));
            }
        }

        public string Text { get; set; }

        public Key Key { get; set; }
        public List<Key> Modifiers { get; set; } = new List<Key>();

        #endregion
        
        #region Implemented Proeprties
        
        public string Name { get; set; } = "Keystroke";
        public PackIconKind Icon { get; set; } = PackIconKind.KeyboardOutline;
        public Guid UUID { get; set; } = new Guid("E6BF1F32-D749-4DF6-AE38-235C1B021AE9");

        #endregion
        
        #region Constructor

        public KeystrokeFunctionViewModel()
        {
            EnumDisplayHelper.AddItems(Functions);

            SelectedFunction = Functions[0];
        }
        
        #endregion

        #region Implemented Methods
        
        public bool AddClicked(int index)
        {
            var function = (IButtonFunction)null;
            switch (SelectedFunction.EnumValue)
            {
                case KeystrokeMode.KeyPress:
                    function = new KeystrokeFunction(index, Key, Modifiers.ToArray(), Guid.NewGuid());
                    break;
                case KeystrokeMode.TextMode:
                    function = new KeystrokeFunction(index, Text, Guid.NewGuid());
                    break;
            }
            ButtonUtil.AddButton(index, function);
            return true;
        }
        
        #endregion
        
        #region Public Methods

        public void SetVisibilityFor(KeystrokeMode mode)
        {
            switch (mode)
            {
                case KeystrokeMode.KeyPress:
                    KeystrokeVisibility = true;
                    TextVisibility = false;
                    break;
                
                case KeystrokeMode.TextMode:
                    KeystrokeVisibility = false;
                    TextVisibility = true;
                    break;
            }
        }

        public void DisplayKeystroke()
        {
            KeystrokeText = KeyUtil.FormatKeys(Key, Modifiers.ToArray());
        }

        #endregion
        
        #region Private Events

        public void KeyDown(KeyEventArgs eventArgs)
        {
            var pressedKey = eventArgs.Key == Key.System ? eventArgs.SystemKey : eventArgs.Key;
            if (pressedKey == Key.LWin || pressedKey == Key.RWin)
                eventArgs.Handled = false;
            if (KeyUtil.IsModifierKey(pressedKey))
            {
                if (!Modifiers.Contains(pressedKey))
                    Modifiers.Add(pressedKey);
            }
            else
                Key = pressedKey;
            DisplayKeystroke();
        }

        public void ClearClick()
        {
            Modifiers.Clear();
            Key = default;
            DisplayKeystroke();
        }
        
        #endregion
        
        #region Property Changed 

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}