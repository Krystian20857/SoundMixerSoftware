using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using NAudio.CoreAudioApi;
using NLog;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Buttons.Functions;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Helpers.SliderConverter;
using SoundMixerSoftware.Helpers.SliderConverter.Converters;
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

        private int _volumeIn;
        private bool _muteIn;
        
        private bool _isEditing;
        private string _name;
        private bool _logScale;

        #endregion
        
        #region Events

        public static event EventHandler<VolumeChangedArgs> VolumeChanged; 
        public static event EventHandler<VolumeChangedArgs> MuteChanged; 
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Input volume control. Set volume for view.
        /// </summary>
        public int VolumeIn
        {
            get => _volumeIn;
            set
            {
                _volumeIn = value;
                
                OnPropertyChanged(nameof(VolumeIn));
                OnPropertyChanged(nameof(VolumeLabel));
            }
        }

        /// <summary>
        /// Output volume control. Set actual volume.
        /// </summary>
        public int VolumeOut
        {
            get => _volumeIn;
            set
            {
                _volumeIn = value;
                SessionHandler.SetVolume(Index, (float)Math.Round(_volumeIn / 100.0F, 2), true);
                
                OnPropertyChanged(nameof(VolumeOut));
                OnPropertyChanged(nameof(VolumeLabel));
            }
        }

        /// <summary>
        /// Return current volume value;
        /// </summary>
        public int VolumeLabel => _volumeIn;

        /// <summary>
        /// Input mute control. Set mute for view.
        /// </summary>
        public bool MuteIn
        {
            get => _muteIn;
            set
            {
                _muteIn = value;
                
                OnPropertyChanged(nameof(MuteIn));
                OnPropertyChanged(nameof(MuteLabel));
            }
        }

        /// <summary>
        /// Output mute control. Set actual mute.
        /// </summary>
        public bool MuteOut
        {
            get => _muteIn;
            set
            {
                _muteIn = value;
                
                SessionHandler.SetMute(Index, value, true);
                OnPropertyChanged(nameof(MuteOut));
                OnPropertyChanged(nameof(MuteLabel));
            }
        }

        /// <summary>
        /// Return current mute state;
        /// </summary>
        public bool MuteLabel => _muteIn;

        /// <summary>
        /// Index of current slider.
        /// </summary>
        public int Index { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            } 
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                if (!value)
                {
                    ProfileHandler.SelectedProfile.Sliders[Index].Name = Name;
                    ProfileHandler.SaveSelectedProfile();
                }
                OnPropertyChanged(nameof(IsEditing));
            }
        }

        public bool LogScale
        {
            get => _logScale;
            set
            {
                _logScale = value;
                if (value && !ConverterHandler.HasConverter<LogarithmicConverter>(Index))
                {
                    var converter = ConverterHandler.AddConverter(Index, new LogarithmicConverter(Index, Guid.NewGuid(), (float) Math.E));
                    ProfileHandler.SelectedProfile.Sliders[Index].Converters.Add(converter);
                }
                
                if(!value)
                {
                    var converters = ConverterHandler.Converters[Index];
                    for (var n = 0; n < converters.Count; n++)
                    {
                        var converter = converters[n];
                        if (converter is LogarithmicConverter)
                        {
                            ConverterHandler.RemoveConverter(Index, converter);
                            ProfileHandler.SelectedProfile.Sliders[Index].Converters.RemoveAt(n);
                        }
                    }
                }
                ProfileHandler.SaveSelectedProfile();
                OnPropertyChanged(nameof(LogScale));
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
        
        #region Constructor

        /// <summary>
        /// Create slider model instance and subscribe events.
        /// </summary>
        public SliderModel()
        {
            VolumeIn = 55;
            foreach (var sessionEnum in SessionHandler.SessionEnumerators)
                sessionEnum.Value.VolumeChanged += SessionEnumeratorOnVolumeChanged;

            SessionHandler.DeviceEnumerator.DeviceVolumeChanged += DeviceEnumeratorOnDeviceVolumeChanged;
        }

        static SliderModel()
        {
            MuteChanged += (sender, args) =>
            {
                var sliderModel = sender as SliderModel;
                var sliderIndex = sliderModel.Index;
                if (MuteFunction.SliderMute.ContainsKey(sliderIndex))
                {
                    var buttonIndexList = MuteFunction.SliderMute[sliderIndex];
                    foreach(var buttonIndex in buttonIndexList)
                        DeviceNotifier.LightButton(unchecked((byte)buttonIndex), sliderModel.MuteLabel);
                }
            };
        }

        /// <summary>
        /// On device volume change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceEnumeratorOnDeviceVolumeChanged(object sender, VolumeChangedArgs e)
        {
            //Device invocation can only happen in thread where COM instance happened otherwise exception occurs and a lot of weird stuff happen.
            Execute.OnUIThread(() =>
            {
                var device = sender as MMDevice;
                var deviceID = string.Copy(device.ID);
                if (Applications.OptimizedAny(x => x.ID == deviceID ||
                                          (x.SessionMode == SessionMode.DefaultInputDevice && deviceID == SessionHandler.DeviceEnumerator.DefaultInputID) ||
                                          (x.SessionMode == SessionMode.DefaultOutputDevice && deviceID == SessionHandler.DeviceEnumerator.DefaultOutputID)))
                {
                    VolumeIn = (int) Math.Round(e.Volume * 100.0F);
                    MuteIn = e.Mute;
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
            if (Applications.OptimizedAny(x => x.ID == sessionControl.GetSessionIdentifier))
            {
                VolumeIn = (int) Math.Round(e.Volume * 100.0F);
                MuteIn = e.Mute;
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
            switch (propertyName)
            {
                case nameof(VolumeIn):
                    VolumeChanged?.Invoke(this, new VolumeChangedArgs(VolumeIn, MuteIn, false));
                    break;
                case nameof(MuteIn):
                    MuteChanged?.Invoke(this, new VolumeChangedArgs(VolumeIn, MuteIn, false));
                    break;
            }
        }

        #endregion
        
    }
}