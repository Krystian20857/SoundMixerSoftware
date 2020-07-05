using System;
using Caliburn.Micro;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Buttons.Functions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class MuteButtonViewModel : IButtonAddModel
    {
        #region Private Fields
        
        private EnumDisplayModel<MuteTask> _selectedFunction;
        
        #endregion
        
        #region Public Properties

        public BindableCollection<EnumDisplayModel<MuteTask>> Functions { get; set; } = new BindableCollection<EnumDisplayModel<MuteTask>>();

        public EnumDisplayModel<MuteTask> SelectedFunction
        {
            get => _selectedFunction;
            set => _selectedFunction = value; 
        }
        
        #endregion

        #region Implemented Properties

        public string Name { get; set; } = "Mute Control";
        
        #endregion
        
        #region Constructor

        public MuteButtonViewModel()
        {
            var enumNames = Enum.GetNames(typeof(MuteTask));
            foreach (var enumName in enumNames)
            {
                var mediaModel = new EnumDisplayModel<MuteTask>
                {
                    EnumValue = (MuteTask)Enum.Parse(typeof(MuteTask), enumName)
                };
                Functions.Add(mediaModel);
            }

            SelectedFunction = Functions[0];
        }
        
        #endregion
        
        #region Implemented Metods
        
        public bool AddClicked(int index)
        {
            var function = ButtonHandler.AddFunction(index, new MuteFunction(index, SelectedFunction.EnumValue, Guid.NewGuid()));
            ProfileHandler.SelectedProfile.Buttons[index].Functions.Add(function);
            ProfileHandler.SaveSelectedProfile();
            return true;
        }
        
        #endregion
    }
}