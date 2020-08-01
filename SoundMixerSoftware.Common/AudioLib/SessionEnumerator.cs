using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NLog;
using SoundMixerSoftware.Common.Threading;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Win32.Threading;

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
        private List<AudioSessionControl> _registeredSessions = new List<AudioSessionControl>();
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
            _device.AudioSessionManager.OnSessionCreated += OnSessionCreated;
            for (var n = 0; n < AudioSessions.Count; n++)
            {
                var session = AudioSessions[n];
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
            return AudioProcesses.Where(x => x.process.ProcessName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public AudioSessionControl GetById(string sessionId)
        {
            for (var n = 0; n < AudioSessions.Count; n++)
            {
                var session = AudioSessions[n];
                try
                {
                    if (!ProcessUtils.IsAlive((int) session.GetProcessID))
                    {
                        session.Dispose();
                        continue;
                    }

                    if (session.GetSessionIdentifier.Equals(sessionId, StringComparison.InvariantCultureIgnoreCase))
                        return session;
                }
                catch (Exception exception)
                {
                    if(exception is COMException)
                        Logger.Error($"Last win32 error:{Marshal.GetLastWin32Error()}");
                    Logger.Error(exception);
                }
            }

            return null;
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
            eventClient.SessionDisconnected += EventClientOnSessionDisconnected;
            eventClient.StateChanged += EventClientOnStateChanged;
            eventClient.VolumeChanged += EventClientOnVolumeChanged;
            eventClient.ChannelVolumeChanged += EventClientOnChannelVolumeChanged;
            eventClient.DisplayNameChanged += EventClientOnDisplayNameChanged;
            eventClient.GroupingParamChanged += EventClientOnGroupingParamChanged;
            eventClient.IconPathChanged += EventClientOnIconPathChanged;
            session.RegisterEventClient(eventClient);
            _registeredSessions.Add(session);
            var exitHandler = new ExitHandler((int)session.GetProcessID, session.GetSessionIdentifier, ProcessWatcher);
            exitHandler.SessionExited += (exitSender, id) =>
            {
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

            foreach (var session in _registeredSessions)
            {
                session.UnRegisterEventClient(null);
                session.Dispose();
            }

            _registeredSessions.Clear();

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