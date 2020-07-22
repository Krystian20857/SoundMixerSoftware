using System.Windows.Threading;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class SessionSlider : IVirtualSlider
    {
        #region Private Fields

        private AudioSessionControl _session;
        
        private Dispatcher _dispatcher =Dispatcher.CurrentDispatcher;

        #endregion

        #region Public Properties

        public float Volume
        {
            get => _dispatcher.Invoke(()=> _session.SimpleAudioVolume.Volume);
            set => SetVolumeInternal(value);
        }

        public bool IsMute
        {
            get => _dispatcher.Invoke(()=> _session.SimpleAudioVolume.Mute);
            set => SetMuteInternal(value);
        }
        public bool IsMasterVolume => false;
        public SliderType SliderType => SliderType.SESSION;
        public string SessionID => _dispatcher.Invoke(() => _session.GetSessionIdentifier);

        #endregion

        #region Constructor

        public SessionSlider(AudioSessionControl sessionControl)
        {
            _session = sessionControl;
        }
        
        #endregion

        #region Private Events

        internal void SetVolumeInternal(float volume)
        {
            _dispatcher.Invoke(() =>
            {
                try
                {
                    _session.SimpleAudioVolume.Volume = volume;
                }
                finally { }
            });
        }

        internal void SetMuteInternal(bool mute)
        {
            _dispatcher.Invoke(() =>
            {
                try
                {
                    _session.SimpleAudioVolume.Mute = mute;
                }
                finally { }
            });
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _session?.Dispose();
        }

        #endregion

    }
}