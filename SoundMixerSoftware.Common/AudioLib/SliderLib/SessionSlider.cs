using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Threading.Com;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class SessionSlider : IVirtualSlider
    {
        #region Private Fields

        private AudioSessionControl _session;

        #endregion

        #region Public Properties

        public float Volume
        {
            get => ComThread.Invoke(() => _session.SimpleAudioVolume.Volume);
            set => SetVolumeInternal(value);
        }

        public bool IsMute
        {
            get => ComThread.Invoke(() => _session.SimpleAudioVolume.Mute);
            set => SetMuteInternal(value);
        }

        public bool IsMasterVolume => false;
        public SliderType SliderType { get; }

        public string SessionID => ComThread.Invoke(() => _session.GetSessionIdentifier);

        #endregion

        #region Constructor

        public SessionSlider(AudioSessionControl session)
        {
            _session = session;
            SliderType = SliderType.SESSION;
        }

        #endregion

        #region Private Methods

        internal void SetVolumeInternal(float volume)
        {
            try
            {
                ComThread.BeginInvoke(() => _session.SimpleAudioVolume.Volume = volume);
            }finally{}
        }

        internal void SetMuteInternal(bool mute)
        {
            try
            {
                ComThread.BeginInvoke(() => _session.SimpleAudioVolume.Mute = mute);
            }finally{}
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