using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using NLog;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Framework.Audio;
using SoundMixerSoftware.Framework.Device;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.SliderConverter;
using SoundMixerSoftware.Framework.SliderConverter.Converters;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Model use to handling Sliders.
    /// </summary>
    public class SliderModel : INotifyPropertyChanged, IDisposable
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
        public static event EventHandler<MuteChangedArgs> MuteChanged; 
        
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
                SessionHandler.SetVolume(Index, _volumeIn, true);
                
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
                            ProfileHandler.SelectedProfile.Sliders[Index].Converters.RemoveAll(x => x.UUID == converter.UUID);
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
        public BindableCollection<IVirtualSession> Applications { get; set; } = new BindableCollection<IVirtualSession>();
        /// <summary>
        /// Current Selected Session.
        /// </summary>
        public IVirtualSession SelectedApp { get; set; }
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Create slider model instance and subscribe events.
        /// </summary>
        public SliderModel()
        {
            SessionHandler.SessionVolumeChanged += (sender, args) =>
            {
                var index = args.Index;
                if(index != Index) return;
                VolumeIn = (int) Math.Round(args.Volume);
            };

            SessionHandler.SessionMuteChanged += (sender, args) =>
            {
                var index = args.Index;
                if(index != Index) return;
                MuteIn = args.Mute;
            };
        }

        #endregion
        
        #region Dispose
        
        public void Dispose()
        {
            foreach (var session in Applications)
            {
                session.Dispose();
            }
            
            GC.SuppressFinalize(this);
        }

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
                    VolumeChanged?.Invoke(this, new VolumeChangedArgs(VolumeIn, false, Index));
                    break;
                case nameof(MuteIn):
                    MuteChanged?.Invoke(this, new MuteChangedArgs(MuteIn,false, Index));
                    break;
            }
        }

        #endregion
    }
}