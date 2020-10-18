using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;
using SoundMixerSoftware.Common.Threading;
using SoundMixerSoftware.Win32.Interop.Constant;
using SoundMixerSoftware.Win32.Interop.Enum;
using SoundMixerSoftware.Win32.Interop.Method;

namespace SoundMixerSoftware.Win32.Threading
{
    /// <summary>
    /// https://github.com/Krystian20857/ProcessWatcher
    /// </summary>
    public class ProcessWatcher : IProcessWatcher
    {
        #region Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private Dictionary<int, ProcessWatcherModel> _watchers = new Dictionary<int, ProcessWatcherModel>();
        private Thread _thread;
        private bool _isRunning = true;
        private readonly object _lockObject = new object();

        #endregion

        #region Public Properties

        /// <summary>
        /// Get attached watchers.
        /// </summary>
        public IReadOnlyDictionary<int, ProcessWatcherModel> Watchers => _watchers;
        
        /// <summary>
        /// Check whatever watcher thread is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_thread == null)
                    return false;
                return _thread.IsAlive && _isRunning;
            }
        }

        public TimeSpan WaitPeriod { get; }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Create process watcher instance.
        /// </summary>
        /// <param name="waitPeriod">Refresh period.</param>
        public ProcessWatcher(TimeSpan waitPeriod)
        {
            WaitPeriod = waitPeriod;
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Attach exit event to process.
        /// </summary>
        /// <param name="processId">Id of process.</param>
        /// <param name="processExited">Exit action.</param>
        public void AttachProcessWait(int processId, Action<int> processExited)
        {
            var processHandle = Kernel32.OpenProcess(ProcessAccessFlags.Synchronize, false, processId);
            if (processHandle == IntPtr.Zero || Kernel32.WaitForSingleObject(processHandle, 0) != Kernel32Const.WAIT_TIMEOUT)
            {
                Logger.Debug($"Unable to watch process: {processHandle}.");
                return;
            }

            lock (_lockObject)
            {

                if (_watchers.ContainsKey(processId))
                {
                    var watcherModel = _watchers[processId];
                    watcherModel.ExitActions.Add(processExited);

                    Kernel32.CloseHandle(processHandle);
                    return;
                }

                var model = new ProcessWatcherModel {ProcessId = processId, ProcessHandle = processHandle};
                model.ExitActions.Add(processExited);
                _watchers.Add(processId, model);
            }

            StartWatcherThread();
        }

        /// <summary>
        /// Detach exit event from process.
        /// </summary>
        /// <param name="processId">Id of process.</param>
        /// <returns>Returns true when process watcher is attached.</returns>
        public bool DetachProcessWait(int processId)
        {
            lock (_lockObject)
            {
                if (!_watchers.ContainsKey(processId))
                    return false;
                var watcherToRemove = _watchers[processId];
                Kernel32.CloseHandle(watcherToRemove.ProcessHandle);
                _watchers.Remove(processId);
                return true;
            }
        }

        /// <summary>
        /// Stops watcher thread permanently.
        /// </summary>
        public void StopWatcherThread()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Start watcher thread manually.
        /// </summary>
        public void StartWatcherThread()
        {
            if (IsRunning) return;
            _isRunning = true;
            _thread = new Thread(() =>
            {
                while (_isRunning)
                {
                    var handles = new IntPtr[0];
                    lock (_lockObject)
                    {
                        handles = _watchers.Select(x => x.Value.ProcessHandle).ToArray();
                    }

                    var result = Kernel32.WaitForMultipleObjects((uint)handles.Length, handles, false, (uint)WaitPeriod.TotalMilliseconds);

                    switch (result)
                    {
                        case Kernel32Const.WAIT_ABANDONED:
                        case Kernel32Const.INFINITE:
                            Thread.Sleep((int)WaitPeriod.TotalMilliseconds);
                            break;
                        case Kernel32Const.WAIT_TIMEOUT:
                            break;
                        
                        default:
                            lock (_lockObject)
                            {
                                var handle = handles[result];

                                var model = _watchers.Values.FirstOrDefault(x => x.ProcessHandle == handle);

                                if (model == default)
                                {
                                    Logger.Debug($"Error while getting process watcher data: {handle}");
                                    break; 
                                }
                                
                                foreach (var exitAction in model.ExitActions)
                                {
                                    exitAction(model.ProcessId);
                                }

                                _watchers.Remove(model.ProcessId);
                                
                                if(_watchers.Count == 0)
                                    StopWatcherThread();
                                Kernel32.CloseHandle(model.ProcessHandle);

                            }
                            break;
                    }
                }
                
            }) {IsBackground = true, Name = "Process Watcher Thread"};
            _thread.SetApartmentState(ApartmentState.MTA);
            _thread.Start();
        }

        #endregion
        
        #region Private Methods
        
        
        #endregion

        #region Dispose

        public void Dispose()
        {
            StopWatcherThread();
            GC.SuppressFinalize(this);
        }
        
        #endregion
    }

    public class ProcessWatcherModel
    {
        public int ProcessId { get; set; }
        public IntPtr ProcessHandle { get; set; }
        public List<Action<int>> ExitActions { get; set; } = new List<Action<int>>();
    }
}