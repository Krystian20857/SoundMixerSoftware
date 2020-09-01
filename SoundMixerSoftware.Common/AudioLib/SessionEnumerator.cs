using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NLog;
using SoundMixerSoftware.Common.Threading;
using SoundMixerSoftware.Common.Threading.Com;
using SoundMixerSoftware.Common.Utils;

namespace SoundMixerSoftware.Common.AudioLib
{
    public class SessionEnumerator : IDisposable
    {
        #region Logger

        /// <summary>
        /// Current Class Logger.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Private Fields

        private MMDevice _device;
        private Dictionary<string, AudioSessionControl> _sessions = new Dictionary<string, AudioSessionControl>();
        private List<ExitHandler> _exitHandlers = new List<ExitHandler>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets audio sessions.
        /// </summary>
        public SessionCollection AudioSessions
        {
            get
            {
                _device.AudioSessionManager.RefreshSessions();
                return _device.AudioSessionManager.Sessions;
            }
        }

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

        public IProcessWatcher ProcessWatcher { get; }

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
        /// Occurs when session process quits
        /// </summary>
        public event EventHandler<string> SessionExited;
        
        private void EventClientOnIconPathChanged(object sender, string e)
        {
            IconPathChanged?.Invoke(sender,e );
        }

        private void EventClientOnGroupingParamChanged(object sender, Guid e)
        {
            GroupingParamChanged?.Invoke(sender, e);
        }

        private void EventClientOnDisplayNameChanged(object sender, string e)
        {
            DisplayNameChanged?.Invoke(sender, e);
        }

        private void EventClientOnChannelVolumeChanged(object sender, ChannelVolumeChangedArgs e)
        {
            ChannelVolumeChanged?.Invoke(sender, e);
        }

        private void EventClientOnVolumeChanged(object sender, VolumeChangedArgs e)
        {
            VolumeChanged?.Invoke(sender, e);
        }

        private void EventClientOnStateChanged(object sender, AudioSessionState e)
        {
            StateChanged?.Invoke(sender, e);
        }

        private void EventClientOnSessionDisconnected(object sender, AudioSessionDisconnectReason e)
        {
            //SessionExited?.Invoke(this, (sender as AudioSessionControl)?.GetSessionIdentifier);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create instance of Session Enumerator with specified device;
        /// </summary>
        public SessionEnumerator(MMDevice device, IProcessWatcher processWatcher)
        {
            ProcessWatcher = processWatcher;
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
            var audioSessions = _device.AudioSessionManager.Sessions;
            for (var n = 0; n < audioSessions.Count; n++)
            {
                var session = audioSessions[n];
                if(session.GetSessionIdentifier.Contains("#%b"))
                    continue;
                RegisterEvents(session);
            }
            _device.AudioSessionManager.OnSessionCreated += OnSessionCreated;
        }
        

        /// <summary>
        /// Get audio session from process name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<(Process process, AudioSessionControl session)> GetByProcessName(string name)
        {
            return AudioProcesses.Where(x => x.process.ProcessName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public AudioSessionControl GetById(string sessionId)
        {
            var sessions = AudioSessions;
            for (var n = 0; n < sessions.Count; n++)
            {
                var session = sessions[n];

                if (session.GetSessionIdentifier == sessionId)
                    return session;
            }

            return null;
        }

        public IEnumerable<AudioSessionControl> GetSessions(string sessionId)
        {
            var sessions = AudioSessions;
            for (var n = 0; n < sessions.Count; n++)
            {
                var session = sessions[n];
                if (session.GetSessionIdentifier == sessionId)
                    yield return session;
            }
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
            var sessionId = session.GetSessionIdentifier;
            
            var eventClient = new AudioSessionEventClient(session);
            eventClient.SessionDisconnected += EventClientOnSessionDisconnected;
            eventClient.StateChanged += EventClientOnStateChanged;
            eventClient.VolumeChanged += EventClientOnVolumeChanged;
            eventClient.ChannelVolumeChanged += EventClientOnChannelVolumeChanged;
            eventClient.DisplayNameChanged += EventClientOnDisplayNameChanged;
            eventClient.GroupingParamChanged += EventClientOnGroupingParamChanged;
            eventClient.IconPathChanged += EventClientOnIconPathChanged;
            session.RegisterEventClient(eventClient);

            if (_sessions.ContainsKey(sessionId))
                _sessions[sessionId] = session;
            else
                _sessions.Add(sessionId, session);
            
            var exitHandler = new ExitHandler((int) session.GetProcessID, sessionId, ProcessWatcher);
            exitHandler.SessionExited += (exitSender, id) =>
            {
                if (_sessions.ContainsKey(sessionId))
                    _sessions.Remove(sessionId);
                SessionExited?.Invoke(session, id);
            };
            _exitHandlers.Add(exitHandler);

        }

        private void OnSessionCreated(object sender, IAudioSessionControl newsession)
        {
            var session = new AudioSessionControl(newsession);
            RegisterEvents(session);
            SessionCreated?.Invoke(_device, session);
        }
        #endregion

        public void Dispose()
        {
            foreach (var session in _sessions)
            {
                var sessionControl = session.Value;
                sessionControl.Dispose();
            }

            _sessions.Clear();
            
            _device.AudioSessionManager.Dispose();

            _device.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class ExitHandler
    {
        #region Public Properties

        public int ProcessId { get; }
        public string SessionId { get; }

        public IProcessWatcher ProcessWatcher { get; }

        #endregion
        
        #region Events
        
        public event EventHandler<string> SessionExited;
        
        #endregion
        
        #region Constructor

        public ExitHandler(int pid, string id, IProcessWatcher processWatcher)
        {
            if (pid == 0)
                return;
            
            ProcessId = pid;
            SessionId = id;

            ProcessWatcher = processWatcher;
            
            ProcessWatcher.AttachProcessWait(pid, processExitId =>
            {
                if(processExitId == ProcessId)
                    SessionExited?.Invoke(this, SessionId);
            });
            
        }
        
        #endregion
    }
    
}