using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Caliburn.Micro;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Helpers.AudioSessions;

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
        /// Index of current slider.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Session Collection.
        /// </summary>
        public BindableCollection<SessionModel> Applications { get; set; } = new BindableCollection<SessionModel>();
        /// <summary>
        /// Current Selected Session.
        /// </summary>
        public SessionModel SelectedApp { get; set; }
        
        #endregion
        
        #region Constructor

        public SliderModel()
        {
            SessionHandler.SessionEnumerator.VolumeChanged += SessionEnumeratorOnVolumeChanged;
        }

        private bool change = true;
        private void SessionEnumeratorOnVolumeChanged(object sender, Common.AudioLib.VolumeChangedArgs e)
        {
            var sessionControl = sender as AudioSessionControl;
            if (Applications.Any(x => x.ID == sessionControl.GetSessionIdentifier))
            {
                Volume = (int) Math.Floor(sessionControl.SimpleAudioVolume.Volume * 100.0F);
                change = false;
            }
        }

        #endregion
        
        #region Property Changed Events
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (change)
            {
                SessionHandler.SetVolume(Index, _volume / 100.0F, true);
                SessionHandler.SetMute(Index, _mute, true);
            }

            change = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}