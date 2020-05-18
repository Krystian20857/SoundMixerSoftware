using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ButtonsViewModel : ITabModel
    {
        #region Private Fields
        
        private BindableCollection<ButtonModel> _buttons = new BindableCollection<ButtonModel>();
        
        #endregion
        
        #region Public Propeties

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

            Buttons.Add(new ButtonModel()
            {
                Name="VolumeUP",
                Function = new BindableCollection<string>()
                {
                    "VolumeUp","VolumeDown","Mute","Next","Prev","PlayPause"
                },
                SelectedItem = "VolumeUp"
            });
        }
        
        #endregion
    }
}