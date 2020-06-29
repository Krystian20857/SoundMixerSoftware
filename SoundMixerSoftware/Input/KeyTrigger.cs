using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace SoundMixerSoftware.Input
{
    // Code base on: https://github.com/Caliburn-Micro/Caliburn.Micro/blob/master/samples/scenarios/Scenario.KeyBinding/Input/KeyTrigger.cs
    public class KeyTrigger : TriggerBase<UIElement>
    {
        #region Dependency Properties
        
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger), null);
        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register("Modifiers", typeof(Key), typeof(KeyTrigger), null);
        
        #endregion
        
        #region Public Properties

        public Key TriggerKey { get; }
        public ModifierKeys Modifiers { get; }

        #endregion
        
        #region Constructor
        
        public KeyTrigger(Key triggerKey, ModifierKeys modifiers)
        {
            TriggerKey = triggerKey;
            Modifiers = modifiers;
        }
        
        #endregion
        
        #region Overriden Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObjectOnKeyDown;
        }

        #endregion

        #region Private Events
        
        private void AssociatedObjectOnKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            if(key == TriggerKey && GetRealModifiers(e.Key, Modifiers) == Keyboard.Modifiers)
                InvokeActions(e);
        }

        #endregion
        
        #region Public Static Methods

        public static ModifierKeys GetRealModifiers(Key key, ModifierKeys modifiers)
        {
            switch (key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    modifiers |= ModifierKeys.Shift;
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    modifiers |= ModifierKeys.Control;
                    break;
                case Key.LeftAlt:
                case Key.RightAlt:
                    modifiers |= ModifierKeys.Alt;
                    break;
            }

            return modifiers;
        }
        
        #endregion
    }
}