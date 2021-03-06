﻿using System;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Buttons.Functions;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.Utils;
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
        public PackIconKind Icon { get; set; } = PackIconKind.VolumeMedium;
        public Guid UUID { get; set; } = new Guid("04ABC847-FBE5-45AD-B2DF-53752A1EDA1A");

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
            var function = new VolumeFunction(index, Guid.NewGuid(), sliderIndex, Volume);
            ButtonUtil.AddButton(index, function);
            return true;
        }
        
        #endregion
    }
}