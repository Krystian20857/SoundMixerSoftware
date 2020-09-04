﻿﻿using System;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace SoundMixerSoftware.Common.AudioLib
{
    public class AudioSessionEventClient : IAudioSessionEventsHandler, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Current session.
        /// </summary>
        private AudioSessionControl _session;
        
        #endregion
        
        #region Events

        /// <summary>
        /// Fires when volume or muting state changed.
        /// </summary>
        public event EventHandler<VolumeChangedArgs> VolumeChanged;
        /// <summary>
        /// Fires when display name changed.
        /// </summary>
        public event EventHandler<string> DisplayNameChanged;
        /// <summary>
        /// Fires when icon path changed.
        /// </summary>
        public event EventHandler<string> IconPathChanged;
        /// <summary>
        /// Fires when channel volume changed
        /// </summary>
        public event EventHandler<ChannelVolumeChangedArgs> ChannelVolumeChanged;
        /// <summary>
        /// Fires when grouping parameter change.
        /// </summary>
        public event EventHandler<Guid> GroupingParamChanged;
        /// <summary>
        /// Fires when session state changed.
        /// </summary>
        public event EventHandler<AudioSessionState> StateChanged;
        /// <summary>
        /// Fires when session disconnected.
        /// </summary>
        public event EventHandler<AudioSessionDisconnectReason> SessionDisconnected;
        
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
            if(_session == null)
                return;
            VolumeChanged?.Invoke(_session, new VolumeChangedArgs(volume, isMuted, false));
        }

        public void OnDisplayNameChanged(string displayName)
        {
            if(_session == null)
                return;
            DisplayNameChanged?.Invoke(_session, displayName);
        }

        public void OnIconPathChanged(string iconPath)
        {
            if(_session == null)
                return;
            IconPathChanged?.Invoke(_session, iconPath);
        }

        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex)
        {
            if(_session == null)
                return;
            ChannelVolumeChanged?.Invoke(_session, new ChannelVolumeChangedArgs(channelCount, newVolumes, channelIndex));
        }

        public void OnGroupingParamChanged(ref Guid groupingId)
        {
            if(_session == null)
                return;
            GroupingParamChanged?.Invoke(_session, groupingId);
        }

        public void OnStateChanged(AudioSessionState state)
        {
            if(_session == null)
                return;
            StateChanged?.Invoke(_session, state);
        }

        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            if(_session == null)
                return;
            SessionDisconnected?.Invoke(_session, disconnectReason);
        }
        
        #endregion

        public void Dispose()
        {
            _session?.Dispose();
            _session = null;
        }
    }
}