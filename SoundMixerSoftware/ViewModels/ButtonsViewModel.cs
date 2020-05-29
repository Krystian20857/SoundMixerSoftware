using System;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of buttons item in main window.
    /// </summary>
    public class ButtonsViewModel : ITabModel
    {
        #region Private Fields
        
        private BindableCollection<ButtonModel> _buttons = new BindableCollection<ButtonModel>();
        
        #endregion
        
        #region Public Propeties

        /// <summary>
        /// Collection of ListView models.
        /// </summary>
        public BindableCollection<ButtonModel> Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
            }
        }

        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion
        
        #region Constructor

        public ButtonsViewModel()
        {
            Name = "Buttons";
            Icon = PackIconKind.GestureTapButton;
            
            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;
            
            ButtonHandler.Initialize();
            CreateButtons();
        }

        #endregion
        
        #region Private Events
        
        private void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            CreateButtons();
        }

        public void FunctionChanged(object sender)
        {
            if (!(sender is ButtonModel model)) return;
            var function = EnumNameConverter.GetValue<ButtonFunction>(model.SelectedItem);
            ProfileHandler.SelectedProfile.Buttons[Buttons.IndexOf(model)].Function = function;
            ProfileHandler.ProfileManager.Save(ProfileHandler.SelectedGuid);
        }
        
        #endregion
        
        #region Private Methods

        private void CreateButtons()
        {
            var enumValueNames = EnumNameConverter.GetNames(typeof(ButtonFunction));
            var buttons = ProfileHandler.SelectedProfile.Buttons;
            
            Buttons.Clear();
            for (var n = 0; n < ProfileHandler.SelectedProfile.ButtonCount; n++)
            {
                var buttonModel = new ButtonModel
                {
                    Name = $"Button {n + 1}",
                    Function = new BindableCollection<string>(enumValueNames),
                };
                if (buttons.Count <= n)
                {
                    buttons.Add(new ButtonStruct
                    {
                        Index = n,
                        Function = ButtonFunction.NoFunction
                    });
                    ProfileHandler.ProfileManager.Save(ProfileHandler.SelectedGuid);
                }

                var function = buttons[n].Function;
                buttonModel.SelectedItem = buttonModel.Function[(int) function];
                Buttons.Add(buttonModel);
            }
        }
        
        #endregion
    }
}