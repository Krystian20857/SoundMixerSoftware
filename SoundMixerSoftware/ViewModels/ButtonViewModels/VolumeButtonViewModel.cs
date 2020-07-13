using System;
using Caliburn.Micro;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Buttons.Functions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class VolumeButtonViewModel : IButtonAddModel
    {
        #region Private Fields
        
        private string _selectedSlider;
        
        #endregion
        
        #region Public Properties

        public int Volume { get; set; }

        public BindableCollection<string> Sliders { get; set; } = new BindableCollection<string>();

        public string SelectedSlider
        {
            get => _selectedSlider;
            set => _selectedSlider = value;
        }
        
        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; } = "Volume Control";
        
        #endregion
        
        #region Constructor

        public VolumeButtonViewModel()
        {
            ProfileHandler.ProfileChanged += (sender, args) => Initialize();
            Initialize();
        }
        
        #endregion
        
        #region Public Methods
        
        public void Initialize()
        {
            var sliders = ProfileHandler.SelectedProfile.Sliders;
            var sliderCount = ProfileHandler.SelectedProfile.SliderCount;
            for (var n = 0; n < sliderCount; n++)
            {
                if (n >= sliders.Count)
                    break;
                Sliders.Add($"{sliders[n].Name}(#{n + 1})");
            }

            if (sliders.Count > 0)
                SelectedSlider = Sliders[0];
        }
        
        #endregion
        
        #region Implemented Methods
        
        public bool AddClicked(int index)
        {
            var sliderIndex = Sliders.IndexOf(SelectedSlider);
            if (sliderIndex == -1)
                return false;
            var button = new VolumeFunction(index, Guid.NewGuid(), sliderIndex, Volume);
            var buttonStruct = ButtonHandler.AddFunction(index, button);
            ProfileHandler.SelectedProfile.Buttons[index].Functions.Add(buttonStruct);
            ProfileHandler.SaveSelectedProfile();
            return true;
        }
        
        #endregion
    }
}