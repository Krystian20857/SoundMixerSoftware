using System;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace SoundMixerSoftware.Common.AudioLib
{
    public class AudioSessionEventClient : IAudioSessionEventsHandler
    {
        #region Private Fields

        /// <summary>
        /// Current session.
        /// </summary>
        private readonly AudioSessionControl _session;
        
        #endregion
        
        #region Events

        /// <summary>
        /// Fires when volume or muting state changed.
        /// </summary>
        public event Action<AudioSessionControl, float, bool> VolumeChanged;
        /// <summary>
        /// Fires when display name changed.
        /// </summary>
        public event Action<AudioSessionControl, string> DisplayNameChanged;
        /// <summary>
        /// Fires when icon path changed.
        /// </summary>
        public event Action<AudioSessionControl, string> IconPathChanged;
        /// <summary>
        /// Fires when channel volume changed
        /// </summary>
        public event Action<AudioSessionControl, uint, IntPtr, uint> ChannelVolumeChanged;
        /// <summary>
        /// Fires when grouping parameter change.
        /// </summary>
        public event Action<AudioSessionControl, Guid> GroupingParamChanged;
        /// <summary>
        /// Fires when session state changed.
        /// </summary>
        public event Action<AudioSessionControl, AudioSessionState> StateChanged;
        /// <summary>
        /// Fires when session disconnected.
        /// </summary>
        public event Action<AudioSessionControl, AudioSessionDisconnectReason> SessionDisconnected;
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Create event client instance with specified session.
        /// </summary>
        /// <param name="session"></param>
        public AudioSessionEventClient(AudioSessionControl session)
        {
            _session = session;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public void OnVolumeChanged(float volume, bool isMuted)
        {
            VolumeChanged?.Invoke(_session, volume, isMuted);
        }

        public void OnDisplayNameChanged(string displayName)
        {
            DisplayNameChanged?.Invoke(_session, displayName);
        }

        public void OnIconPathChanged(string iconPath)
        {
            IconPathChanged?.Invoke(_session, iconPath);
        }

        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex)
        {
            ChannelVolumeChanged?.Invoke(_session, channelCount, newVolumes, channelCount);
        }

        public void OnGroupingParamChanged(ref Guid groupingId)
        {
            GroupingParamChanged?.Invoke(_session, groupingId);
        }

        public void OnStateChanged(AudioSessionState state)
        {
            StateChanged?.Invoke(_session, state);
        }

        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            SessionDisconnected?.Invoke(_session, disconnectReason);
        }
        
        #endregion
    }
}