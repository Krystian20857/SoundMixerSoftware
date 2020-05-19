using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Model use to handling Sliders.
    /// </summary>
    public class SliderModel : INotifyPropertyChanged
    {
        #region Private Fields

        private bool _mute;
        private int _volume;
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Mute of current session.
        /// </summary>
        public int Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        /// <summary>
        /// Mute state of current session.
        /// </summary>
        public bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                OnPropertyChanged(nameof(Mute));
            }
        }

        /// <summary>
        /// Session Collection.
        /// </summary>
        public BindableCollection<SessionModel> Applications { get; set; } = new BindableCollection<SessionModel>();
        /// <summary>
        /// Current Selected Session.
        /// </summary>
        public SessionModel SelectedApp { get; set; }
        
        #endregion
        
        #region Property Changed Events
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}