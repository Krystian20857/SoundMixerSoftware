using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using SoundMixerSoftware.Common.Collection;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Interop.Wrapper;
using SoundMixerSoftware.Resource.Image;

namespace SoundMixerSoftware.Framework.Audio.VirtualSessions
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ProcessSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Constant

        public const string KEY = "process_session";
        public const string EXECUTABLE_KEY = "ExecName";
        public const string NAME_KEY = "Name";
        
        private const int SESSION_CAPACITY = 15;
        
        #endregion
        
        #region Private Fields
        
        private readonly ConcurrentList<IAudioSession> _sessions = new ConcurrentList<IAudioSession>(SESSION_CAPACITY);
        private readonly List<IDisposable> toDispose = new List<IDisposable>();

        #endregion
        
        #region Implemented Methods

        public string Key { get; } = KEY;
        public string DisplayName { get; set; }
        public string ID => ExecutableName;
        public int Index { get; set; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }

        public SessionState State
        {
            get => _sessions.Count > 0 ? SessionState.ACTIVE : SessionState.EXITED;
            set{}
        }

        public float Volume
        {
            set => _sessions.ForEach(x => x.SetVolumeAsync(value));
            get => State == SessionState.ACTIVE ? (float)_sessions[0].Volume : 0.0F;
        }

        public bool IsMute
        {
            set => _sessions.ForEach(x => x.SetMuteAsync(value));
            // ReSharper disable once SimplifyConditionalTernaryExpression
            get => State == SessionState.ACTIVE ? _sessions[0].IsMuted : false;
        }
        
        #endregion
        
        #region Properties

        private IAudioSession FirstSession => _sessions.FirstOrDefault(x => ProcessWrapper.IsAlive(x.ProcessId));
        // ReSharper disable once MemberCanBePrivate.Global
        public string ExecutableName { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public string RawName { get; set; }

        #endregion
        
        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Constructor

        // ReSharper disable once MemberCanBePrivate.Global
        public ProcessSession(string execName, string rawName, Guid uuid)
        {
            UUID = uuid;
            ExecutableName = execName;
            RawName = rawName;

            SessionHandler.SessionCreated += AddSession;
            SessionHandler.SessionExited += RemoveSession;

            foreach (var session in GetSessionByExecName(execName))
            {
                AddSession(session);
            }
            
            UpdateView();
        }

        public ProcessSession(int index, string execPath, string rawName, Guid uuid) : this(execPath, rawName, uuid)
        {
            Index = index;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var container = new Dictionary<object, object>();
            container.Add(EXECUTABLE_KEY, ExecutableName);
            container.Add(NAME_KEY, RawName);
            return container;
        }
        
        #endregion
        
        #region Private Methods

        private void UpdateView()
        {
            TaskUtil.BeginInvokeDispatcher(() =>
            {
                try
                {
                    var session = FirstSession;
                    if (State == SessionState.ACTIVE && session != default)
                    {
                        try
                        {
                            using (var process = Process.GetProcessById(session.ProcessId))
                            {
                                Image = process.GetMainWindowIcon().ToImageSource();
                                DisplayName = $"{process.GetPreciseName()}";
                            }
                        }
                        catch (Exception) {} //process exited.
                    }
                    else
                    {
                        DisplayName = $"{RawName}(Not Active)";
                        Image = Images.FailedEmbed;
                    }
                }
                finally
                {
                    OnPropertyChanged(nameof(DisplayName));
                    OnPropertyChanged(nameof(Image));
                }
            });
        }

        private void AddSession(IAudioSession session)
        {
            if(session.IsSystemSession) return;
            
            var executablePath = SessionHandler.GetSessionExec(session);
            var executableName = Path.GetFileName(executablePath);
            // ReSharper disable once InvertIf
            if (executableName == ExecutableName)
            {
                _sessions.Add(session);
                toDispose.Add(session.VolumeChanged.Subscribe(VolumeChangedCallback));
                toDispose.Add(session.MuteChanged.Subscribe(MuteChangedCallback));
                UpdateView();
            }
        }

        private void RemoveSession(IAudioSession session)
        {
            if(session.IsSystemSession) return;
            
            var executablePath = SessionHandler.GetSessionExec(session);
            var executableName = Path.GetFileName(executablePath);
            if (executableName != ExecutableName) return;
            if(_sessions.Remove(session))
                UpdateView();
        }

        private void VolumeChangedCallback(SessionVolumeChangedArgs args)
        {
            VolumeChange?.Invoke(this, new VolumeChangedArgs((float)args.Volume, false, Index));
        }
        
        private void MuteChangedCallback(SessionMuteChangedArgs args)
        {
            MuteChanged?.Invoke(this, new MuteChangedArgs(args.IsMuted, false, Index));
        }

        private static IEnumerable<IAudioSession> GetSessionByExecName(string execName)
        {
            return SessionHandler.GetAllSessions().Where(x => Path.GetFileName(SessionHandler.GetSessionExec(x)) == execName);
        }

        #endregion
        
        #region Dispose
        
        public void Dispose()
        {
            _sessions.Clear();
            foreach (var disposable in toDispose)
            {
                disposable.Dispose();
            }
        }
        
        #endregion
        
        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }

    public class ProcessSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            var execPath = container.ContainsKey(ProcessSession.EXECUTABLE_KEY) ? container[ProcessSession.EXECUTABLE_KEY] : string.Empty;
            var name = container.ContainsKey(ProcessSession.NAME_KEY) ? container[ProcessSession.NAME_KEY] : string.Empty; 
            return new ProcessSession(index, execPath.ToString(), name.ToString(), uuid);
        }
    }
}