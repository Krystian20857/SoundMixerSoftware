using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using NAudio.CoreAudioApi;
using NLog;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Helpers.AudioSessions;
using LogManager = NLog.LogManager;
using VolumeChangedArgs = SoundMixerSoftware.Common.AudioLib.VolumeChangedArgs;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Model use to handling Sliders.
    /// </summary>
    public class SliderModel : INotifyPropertyChanged
    {
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private bool _mute;
        private int _volume;
        /// <summary>
        /// When true volume changed can happen.
        /// </summary>
        private bool change;

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
                if (change)
                    SessionHandler.SetVolume(Index, (float)Math.Round(_volume / 100.0F,2), true);
                change = true;
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
                if (change)
                    SessionHandler.SetMute(Index, _mute, true);
                change = true;
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

        /// <summary>
        /// Create slider model instance and subscribe events.
        /// </summary>
        public SliderModel()
        {
            foreach(var sessionEnum in SessionHandler.SessionEnumerators)
                sessionEnum.Value.VolumeChanged += SessionEnumeratorOnVolumeChanged;
            SessionHandler.DeviceEnumerator.DeviceVolumeChanged += DeviceEnumeratorOnDeviceVolumeChanged;
        }

        /// <summary>
        /// On device volume change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceEnumeratorOnDeviceVolumeChanged(object sender, VolumeChangedArgs e)
        {
            //Device invocation can only happen in main thread(ui thread) otherwise exception occurs.
            Execute.OnUIThread(() =>
            {
                var device = sender as MMDevice;
                if (Applications.Any(x => x.ID == device.ID ||
                                          (x.SessionMode == SessionMode.DefaultInputDevice && device.ID == SessionHandler.DeviceEnumerator.DefaultInput.ID) ||
                                          (x.SessionMode == SessionMode.DefaultOutputDevice && device.ID == SessionHandler.DeviceEnumerator.DefaultOutput.ID)))
                {
                    change = false;
                    var volume = (int) Math.Floor(e.Volume * 100.0F);
                    Volume = volume;
                    if(volume != 0)
                        Mute = e.Mute;
                }
            });
        }

        /// <summary>
        /// On session volume change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SessionEnumeratorOnVolumeChanged(object sender, VolumeChangedArgs e)
        {
            var sessionControl = sender as AudioSessionControl;
            if (Applications.Any(x => x.ID == sessionControl.GetSessionIdentifier))
            {
                change = false;
                var volume = (int) Math.Floor(e.Volume * 100.0F);
                Volume = volume;
                Mute = e.Mute;
            }
        }

        #endregion
        
        #region Private Methods

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