using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Buttons.Functions;
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
        
        private IWindowManager _windowManager = new WindowManager();
        
        #endregion
        
        #region Public Propeties

        public BindableCollection<ButtonModel> Buttons { get; set; } = new BindableCollection<ButtonModel>();

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
            CreateButtons();
            
            //ButtonHandler.RegisterCreator("media_func", new MediaFunctionCreator());
            //ButtonHandler.CreateButtons();
            //ButtonHandler.HandleButton(2);
        }

        #endregion
        
        #region Private Events
        
        private void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            CreateButtons();
        }

        public void AddClick(object sender)
        {
            var buttonModel = sender as ButtonModel;
            var addViewModel = IoC.Get<ButtonAddViewModel>();
            addViewModel.Index = buttonModel.Index;
            _windowManager.ShowWindowAsync(addViewModel);
        }
        
        
        public void RemoveClick(object sender)
        {
            
        }
        
        
        public void LightClicked(object sender)
        {
            
        }

        public void EditNameClicked(object sender)
        {
            var buttonModel = sender as ButtonModel;
            buttonModel.IsEditing = !buttonModel.IsEditing;
        }

        public void ConfirmEdit(object sender)
        {
            var buttonModel = sender as ButtonModel;
            if(buttonModel.IsEditing)
                buttonModel.IsEditing = !buttonModel.IsEditing;
        }
        
        #endregion
        
        #region Private Methods

        private void CreateButtons()
        {
            Buttons.Clear();
            var buttonCount = ProfileHandler.SelectedProfile.ButtonCount;
            var buttons = ProfileHandler.SelectedProfile.Buttons;
            var modified = false;

            if (buttonCount > buttons.Count)
            {
                for (var n = buttons.Count; n < buttonCount; n++)
                    buttons.Add(new ButtonStruct
                    {
                        Name = $"Button {n + 1}"
                    });
                modified = true;
            }

            for (var n = 0; n < buttonCount; n++)
            {
                var button = buttons[n];
                if (string.IsNullOrEmpty(button.Name))
                {
                    button.Name = $"Button {n + 1}";
                    modified = true;
                }
                var buttonModel = new ButtonModel
                {
                    Name = button.Name,
                    Index = n,
                };
                Buttons.Add(buttonModel);
            }
            
            if(modified)
                ProfileHandler.SaveSelectedProfile();
            
            ButtonHandler.CreateButtons();
        }
        
        #endregion
    }
}