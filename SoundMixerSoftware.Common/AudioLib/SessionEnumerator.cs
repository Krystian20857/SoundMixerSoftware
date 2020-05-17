﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NLog;

namespace SoundMixerSoftware.Common.AudioLib
{
    public class SessionEnumerator
    {
        #region Logger

        /// <summary>
        /// Current Class Logger.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Private Fields

        private MMDevice _device;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets audio sessions.
        /// </summary>
        public SessionCollection AudioSessions => _device.AudioSessionManager.Sessions;

        /// <summary>
        /// Gets and sets currently operating device.
        /// </summary>
        public MMDevice ParentDevice
        {
            set => SetDevice(value);
            get => _device;
        }

        /*
        /// <summary>
        /// Event Session Event Client.
        /// </summary>
        public AudioSessionEventClient EventClient { get; private set; }
        */

        /// <summary>
        /// Gets processes associated with audio sessions 
        /// </summary>
        public IEnumerable<(Process process, AudioSessionControl session)> AudioProcesses
        {
            get
            {
                for (var n = 0; n < AudioSessions.Count; n++)
                {
                    var session = AudioSessions[n];
                    if (!session.IsSystemSoundsSession)
                        yield return (Process.GetProcessById((int) session.GetProcessID), session);
                }
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Fires when new session has created.
        /// </summary>
        public event EventHandler<AudioSessionControl> SessionCreated;
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
        /// Create instance of Session Enumerator with specified device;
        /// </summary>
        public SessionEnumerator(MMDevice device)
        {
            SetDevice(device);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set current handling device.
        /// </summary>
        /// <param name="device"></param>
        public void SetDevice(MMDevice device)
        {
            _device = device;
            _device.AudioSessionManager.OnSessionCreated += OnSessionCreated;
            var sessions = AudioSessions;
            for (var n = 0; n < sessions.Count; n++)
            {
                var session = sessions[n];
                RegisterEvents(session);
            }
        }

        /// <summary>
        /// Get audio session from process name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<(Process process, AudioSessionControl session)> GetByProcessName(string name)
        {
            return AudioProcesses.Where((x) =>
                x.process.ProcessName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get audio session by process id
        /// </summary>
        /// <param name="pid">process id</param>
        /// <returns></returns>
        public AudioSessionControl GetByProcessId(int pid)
        {
            for (var n = 0; n < AudioSessions.Count; n++)
                if (AudioSessions[n].GetProcessID == pid)
                    return AudioSessions[n];
            return null;
        }

        public bool ProcessNameExists(string processName)
        {
            return AudioProcesses.Any(x =>
                x.process.ProcessName.Equals(processName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
        
        #region Private Methods
 
        private void RegisterEvents(AudioSessionControl session)
        {
            var eventClient = new AudioSessionEventClient(session);
            eventClient.SessionDisconnected += (sender, args) =>SessionDisconnected.Invoke(sender, args);
            eventClient.StateChanged += (sender, args) => StateChanged?.Invoke(sender, args);
            eventClient.VolumeChanged += (sender, args) => VolumeChanged?.Invoke(sender, args);
            eventClient.ChannelVolumeChanged += (sender, args) => ChannelVolumeChanged?.Invoke(sender, args);
            eventClient.DisplayNameChanged += (sender, args) => DisplayNameChanged?.Invoke(sender, args);
            eventClient.GroupingParamChanged += (sender, args) => GroupingParamChanged?.Invoke(sender, args);
            eventClient.IconPathChanged += (sender, args) => IconPathChanged?.Invoke(sender, args);
            session.RegisterEventClient(eventClient);
        }

        private void OnSessionCreated(object sender, IAudioSessionControl newsession)
        {
            var session = new AudioSessionControl(newsession);
            RegisterEvents(session);
            SessionCreated?.Invoke(_device, session);
        }
        #endregion
    }

}