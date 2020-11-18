using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Buttons.Functions;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.Utils;
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
        public PackIconKind Icon { get; set; } = PackIconKind.VolumeOff;
        public Guid UUID { get; set; } = new Guid("8941E120-F5DA-4877-B589-8834E6313391");

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
            var function = (IButtonFunction) null;
            switch (SelectedFunction.EnumValue)
            {
                case MuteTask.MuteSlider:
                    var sliderIndex = Sliders.IndexOf(SelectedSlider);
                    if (sliderIndex == -1)
                        break;
                    if (ButtonHandler.Buttons[index].Any(x => (x is MuteFunction muteFunction) && muteFunction.SliderIndex == sliderIndex))
                        return false;
                    function = new MuteFunction(index, sliderIndex, Guid.NewGuid());
                    break;
                default:
                    function = new MuteFunction(index, SelectedFunction.EnumValue, Guid.NewGuid());
                    break;
            }
            ButtonUtil.AddButton(index, function);
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