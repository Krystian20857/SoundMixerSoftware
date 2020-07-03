using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SoundMixerSoftware.Helpers.Profile;

namespace SoundMixerSoftware.Helpers.Buttons
{
    public static class ButtonHandler
    {
        #region Private Fields
        
        
        
        #endregion
        
        #region Public Properties

        public static Dictionary<string, IButtonCreator> ButtonRegistry { get; } = new Dictionary<string, IButtonCreator>();
        public static List<List<IButton>> Buttons { get; } = new List<List<IButton>>();

        #endregion
        
        #region Events

        public static event EventHandler<FunctionArgs> FunctionCreated;
        public static event EventHandler<FunctionArgs> FunctionRemoved;
        
        #endregion
        
        #region Constructor

        static ButtonHandler()
        {
            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;
        }

        #endregion
        
        #region Public Static Methods

        /// <summary>
        /// Create all buttons from selected profile.
        /// </summary>
        public static void CreateButtons()
        {
            Buttons.Clear();
            var buttonCount = ProfileHandler.SelectedProfile.ButtonCount;
            Buttons.Capacity = buttonCount;
            for (var n = 0; n < buttonCount; n++)
                Buttons.Add(new List<IButton>());
            var buttons = ProfileHandler.SelectedProfile.Buttons;
            for (var n = 0; n < buttons.Count; n++)
            {
                var buttonStruct = buttons[n];
                if(n >= buttonCount)
                    continue;
                if(buttonStruct.Functions == null)
                    buttonStruct.Functions = new List<ButtonFunction>();
                for (var x = 0; x < buttonStruct.Functions.Count; x++)
                {
                    var function = buttonStruct.Functions[x];
                    AddFunction(n, function);
                }
            }
        }

        /// <summary>
        /// Add function to button.
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="button"></param>
        public static IButton AddFunction(int Index, ButtonFunction button)
        {
            if (!ButtonRegistry.ContainsKey(button.Key) || string.IsNullOrEmpty(button.Key))
                return null;
            var creator = ButtonRegistry[button.Key];
            var iButton = creator.CreateButton(Index, button.Container, button.UUID);
            Buttons[Index].Add(iButton);
            FunctionCreated?.Invoke(null, new FunctionArgs(Index, Buttons[Index].IndexOf(iButton), iButton));
            return iButton;
        }
        
        /// <summary>
        /// Add function to button.
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="button"></param>
        public static ButtonFunction AddFunction(int Index, IButton iButton)
        {
            Buttons[Index].Add(iButton);
            FunctionCreated?.Invoke(null, new FunctionArgs(Index, Buttons[Index].IndexOf(iButton), iButton));
            return new ButtonFunction
            {
                Container = iButton.Save(),
                Key = iButton.Key,
                UUID = iButton.UUID
            };
        }

        /// <summary>
        /// Remove function from button by index of function.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="functionIndex"></param>
        public static void RemoveFunction(int index, int functionIndex)
        {
            var iButton = Buttons[index][functionIndex];
            FunctionRemoved?.Invoke(null, new FunctionArgs(index, Buttons[index].IndexOf(iButton), iButton));
            Buttons[index].Remove(iButton);
        }
        
        /// <summary>
        /// Remove function from button by index of function.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="functionIndex"></param>
        public static void RemoveFunction(int index, IButton iButton)
        {
            FunctionRemoved?.Invoke(null, new FunctionArgs(index, Buttons[index].IndexOf(iButton), iButton));
            Buttons[index].Remove(iButton);
        }

        /// <summary>
        /// Registry button creator.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creator"></param>
        public static void RegisterCreator(string key, IButtonCreator creator)
        {
            if (!ButtonRegistry.ContainsKey(key))
                ButtonRegistry.Add(key, creator);
        }
        
        /// <summary>
        /// Unregister button creator.
        /// </summary>
        /// <param name="key"></param>
        public static void UnregisterCreator(string key)
        {
            if (ButtonRegistry.ContainsKey(key))
                ButtonRegistry.Remove(key);
        }

        public static void HandleKeyDown(int index)
        {
            if (index >= Buttons.Count)
                return;
            var buttons = Buttons[index];
            for (var n = 0; n < buttons.Count; n++)
                buttons[n].ButtonKeyDown(index);
        }
        
        public static void HandleKeyUp(int index)
        {
            if (index >= Buttons.Count)
                return;
            var buttons = Buttons[index];
            for (var n = 0; n < buttons.Count; n++)
                buttons[n].ButtonKeyUp(index);
        }

        #endregion

        #region Private Static Methods

        private static void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            
        }

        #endregion
    }

    public class FunctionArgs : EventArgs
    {
        /// <summary>
        /// Index of button.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Index of function.
        /// </summary>
        public int FunctionIndex { get; set; }
        /// <summary>
        /// Button instance.
        /// </summary>
        public IButton Button { get; set; }

        public FunctionArgs(int index, int functionIndex, IButton button)
        {
            Index = index;
            FunctionIndex = functionIndex;
            Button = button;
        }
    }
}