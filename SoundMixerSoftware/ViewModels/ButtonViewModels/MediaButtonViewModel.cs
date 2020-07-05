using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Buttons.Functions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

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
        
        #endregion
        
        #region Constructor

        public MediaButtonViewModel()
        {
            var enumNames = Enum.GetNames(typeof(MediaTask));
            foreach (var enumName in enumNames)
            {
                var mediaModel = new EnumDisplayModel<MediaTask>()
                {
                    EnumValue = (MediaTask)Enum.Parse(typeof(MediaTask), enumName)
                };
                Functions.Add(mediaModel);
            }

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