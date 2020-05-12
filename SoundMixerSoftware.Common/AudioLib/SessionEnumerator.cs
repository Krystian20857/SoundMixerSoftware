using System;
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

        private Dictionary<AudioSessionControl, AudioSessionState> _lastStates =
            new Dictionary<AudioSessionControl, AudioSessionState>();

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

        /// <summary>
        /// Event Session Event Client.
        /// </summary>
        public AudioSessionEventClient EventClient { get; private set; }

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
        public event Action<MMDevice, AudioSessionControl> SessionCreated;

        /// <summary>
        /// Fires when session has disconnected
        /// </summary>
        public event Action<MMDevice, AudioSessionControl> SessionDisconnected;

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

        public void SetDevice(MMDevice device)
        {
            _device = device;
            _device.AudioSessionManager.OnSessionCreated += OnSessionCreated;
            var sessions = AudioSessions;
            for (var n = 0; n < sessions.Count; n++)
            {
                var session = sessions[n];
                EventClient = new AudioSessionEventClient(session);
                EventClient.StateChanged += OnStateChanged;
                session.RegisterEventClient(EventClient);
            }
        }

        public IEnumerable<(Process process, AudioSessionControl session)> GetByProcessName(string name)
        {
            return AudioProcesses.Where((x) =>
                x.process.ProcessName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public AudioSessionControl GetByProcessId(int pid)
        {
            for (var n = 0; n < AudioSessions.Count; n++)
                if (AudioSessions[n].GetProcessID == pid)
                    return AudioSessions[n];
            return null;
        }

        #endregion

        #region Private Methods

        private void OnSessionCreated(object sender, IAudioSessionControl newsession)
        {
            var session = new AudioSessionControl(newsession);
            session.RegisterEventClient(EventClient);
            SessionCreated?.Invoke(_device, session);
        }

        private void OnStateChanged(AudioSessionControl audioSession, AudioSessionState state)
        {
            if (_lastStates.ContainsKey(audioSession))
            {
                if (state != _lastStates[audioSession] && state == AudioSessionState.AudioSessionStateExpired)
                {
                    _lastStates[audioSession] = state;
                    SessionDisconnected?.Invoke(_device, audioSession);
                }

            }
            else
            {
                _lastStates.Add(audioSession, state);
                if(state == AudioSessionState.AudioSessionStateInactive)
                    SessionDisconnected?.Invoke(_device, audioSession);
            }
        }
        #endregion
    }

}