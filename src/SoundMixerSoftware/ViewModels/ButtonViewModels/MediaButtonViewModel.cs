using System;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Buttons.Functions;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Utils;

namespace SoundMixerSoftware.ViewModels
{
    public class MediaButtonViewModel : IButtonAddModel
    {
        #region Private Fields
        
        private EnumDisplayModel<MediaTask> _selectedFunction;
        
        #endregion
        
        #region Public Properties

        public BindableCollection<EnumDisplayModel<MediaTask>> Functions { get; set; } = new BindableCollection<EnumDisplayModel<MediaTask>>();

        public EnumDisplayModel<MediaTask> SelectedFunction
        {
            get => _selectedFunction;
            set => _selectedFunction = value;
        }

        #endregion
        
        #region Implemented Proeprties
        
        public string Name { get; set; } = "Media Control";
        public PackIconKind Icon { get; set; } = PackIconKind.PlayPause;
        public Guid UUID { get; set; } = new Guid("64B974CF-429B-48E5-BDA0-AE13EC1C9C46");

        #endregion
        
        #region Constructor

        public MediaButtonViewModel()
        {
            EnumDisplayHelper.AddItems(Functions);

            SelectedFunction = Functions[0];
        }
        
        #endregion
        
        #region Implemented Methods
        
        public bool AddClicked(int index)
        {
            var function = ButtonHandler.AddFunction(index, new MediaFunction(index, SelectedFunction.EnumValue, Guid.NewGuid()));
            ProfileHandler.SelectedProfile.Buttons[index].Functions.Add(function);
            ProfileHandler.SaveSelectedProfile();
            return true;
        }
        
        #endregion
    }
}