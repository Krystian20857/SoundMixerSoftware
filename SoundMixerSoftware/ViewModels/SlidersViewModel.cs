using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// Device model of Sliders Tab.
    /// </summary>
    public class SlidersViewModel : PropertyChangedBase, ITabModel
    {
        #region Private Fields
        
        private BindableCollection<SliderModel> _sliders = new BindableCollection<SliderModel>();
        private IWindowManager _windowManager = new WindowManager();
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// All available sliders.
        /// </summary>
        public BindableCollection<SliderModel> Sliders
        {
            get => _sliders;
            set
            {
                _sliders = value;
                NotifyOfPropertyChange(() => Sliders);
            }
        }
        
        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion
        

        #region Constructor
        
        public SlidersViewModel()
        {
            Name = "Sliders";
            Icon = PackIconKind.VolumeSource;
        }

        #endregion
        
        #region Private Events

        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void AddClick(object sender)
        {
            _windowManager.ShowDialog(new SessionAddViewModel());
        }

        /// <summary>
        /// Occurs when Remove Button has Clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void RemoveClick(object sender)
        {
            var model = sender as SliderModel;
            model.Applications.Remove(model.SelectedApp);
        }

        /// <summary>
        /// Occurs when Mute Button has Clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void MuteClick(object sender)
        {
            var model = sender as SliderModel;
            model.Mute = !model.Mute;
        }
        
        #endregion
    }
}