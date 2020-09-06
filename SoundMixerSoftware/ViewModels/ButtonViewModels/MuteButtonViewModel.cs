using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Buttons.Functions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Utils;

namespace SoundMixerSoftware.ViewModels
{
    public class MuteButtonViewModel : IButtonAddModel, INotifyPropertyChanged
    {
        #region Private Fields
        
        private EnumDisplayModel<MuteTask> _selectedFunction;
        private string _selectedSlider;

        #endregion
        
        #region Public Properties

        public BindableCollection<EnumDisplayModel<MuteTask>> Functions { get; set; } = new BindableCollection<EnumDisplayModel<MuteTask>>();

        public EnumDisplayModel<MuteTask> SelectedFunction
        {
            get => _selectedFunction;
            set
            {
                _selectedFunction = value;
                SlidersVisibility = value.EnumValue == MuteTask.MuteSlider;
                OnPropertyChanged(nameof(SlidersVisibility));
            }
        }

        public BindableCollection<string> Sliders { get; set; } = new BindableCollection<string>();

        public string SelectedSlider
        {
            get => _selectedSlider;
            set => _selectedSlider = value;
        }

        public bool SlidersVisibility { get; set; }

        #endregion

        #region Implemented Properties

        public string Name { get; set; } = "Mute Control";
        
        #endregion
        
        #region Constructor

        public MuteButtonViewModel()
        {
            EnumDisplayHelper.AddItems(Functions);

            SelectedFunction = Functions[0];
            ProfileHandler.ProfileChanged += (sender, args) => Initialize();
            Initialize();
        }

        #endregion
        
        #region Implemented Metods
        
        public bool AddClicked(int index)
        {
            var function = (IButton) null;
            switch (SelectedFunction.EnumValue)
            {
                case MuteTask.MuteSlider:
                    var sliderIndex = Sliders.IndexOf(SelectedSlider);
                    if (sliderIndex == -1)
                        break;
                    if (MuteFunction.SliderMute.ContainsKey(sliderIndex))
                        if (MuteFunction.SliderMute[sliderIndex].Contains(index))
                            return false;
                    function = new MuteFunction(index, sliderIndex, Guid.NewGuid());
                    break;
                default:
                    function = new MuteFunction(index, SelectedFunction.EnumValue, Guid.NewGuid());
                    break;
            }
            var buttonStruct = ButtonHandler.AddFunction(index, function);
            ProfileHandler.SelectedProfile.Buttons[index].Functions.Add(buttonStruct);
            ProfileHandler.SaveSelectedProfile();
            return true;
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