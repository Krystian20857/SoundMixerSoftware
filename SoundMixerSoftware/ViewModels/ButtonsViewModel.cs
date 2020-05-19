using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
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