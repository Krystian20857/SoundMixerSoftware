using System;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using NLog;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Framework.Audio.Threading;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Interop.Threading;
using SoundMixerSoftware.Interop.Wrapper;
using ProcessWatcher = SoundMixerSoftware.Framework.Threading.ProcessWatcher;

namespace SoundMixerSoftware.Framework.Audio
{
    public static class SessionHandler
    {
        #region Logger
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private static IDisposable _changeCallback;
        private static IProcessWatcher _processWatcher => ProcessWatcher.DefaultProcessWatcher;
        private static Dictionary<int, string> _execPaths = new Dictionary<int, string>();

        #endregion
        
        #region Public Properties

        public static List<List<IVirtualSession>> Sessions { get;} = new List<List<IVirtualSession>>();
        public static Dictionary<string, IVirtualSessionCreator> Creators { get; } = new Dictionary<string, IVirtualSessionCreator>();

        public static IAudioController AudioController { get;} = new CoreAudioController();
        public static Dictionary<Guid, IAudioSessionController> SessionController { get;} = new Dictionary<Guid, IAudioSessionController>();

        #endregion
        
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler<SessionArgs> VirtualSessionCreated;
        public static event EventHandler<SessionArgs> VirtualSessionRemoved;
        public static event Action<IAudioSession> SessionExited;
        public static event Action<IAudioSession> SessionCreated;
        public static event EventHandler<VolumeChangedArgs> SessionVolumeChanged;
        public static event EventHandler<MuteChangedArgs> SessionMuteChanged;
        public static event Action<IDevice> DeviceAddedCallback;
        public static event Action<IDevice> DeviceRemovedCallback;

        #endregion
        
        #region Constructor
        
        
        #endregion
        
        #region Public Methods
        
        public static void ReloadAll()
        {
            _changeCallback?.Dispose();
            
            foreach (var session in Sessions.SelectMany(x => x.ToList()))
                session.Dispose();
            
            foreach(var device in AudioController.GetDevices(DeviceState.Active))
                DeviceAdded(device);
            
            _changeCallback = AudioController.AudioDeviceChanged.Subscribe(x =>
            {
                switch (x.ChangedType)
                {
                    case DeviceChangedType.DeviceAdded:
                        DeviceAdded(x.Device);
                        break;
                    case DeviceChangedType.DeviceRemoved:
                        DeviceRemove(x.Device);
                        break;
                    case DeviceChangedType.StateChanged:
                        var state = x as DeviceStateChangedArgs;
                        switch (state.State)
                        {
                            case DeviceState.Active:
                                DeviceAdded(x.Device);
                                break;
                            case DeviceState.Disabled:
                            case DeviceState.NotPresent:
                            case DeviceState.Unplugged:
                                DeviceRemove(x.Device);
                                break;
                        }
                        break;
                }
            });
        }

        public static void CreateSessions()
        {
            Sessions.Clear();
            var sliderCount = ProfileHandler.SelectedProfile.SliderCount;
            Sessions.Capacity = sliderCount;
            for (var n = 0; n < sliderCount; n++)
                Sessions.Add(new List<IVirtualSession>());
            var sliders = ProfileHandler.SelectedProfile.Sliders;
            for (var n = 0; n < sliders.Count; n++)
            {
                var slider = sliders[n];
                if (n >= sliderCount)
                    continue;
                if (slider.Sessions == null)
                    slider.Sessions = new List<Session>();
                for (var x = 0; x < slider.Sessions.Count; x++)
                {
                    var session = slider.Sessions[x];
                    AddSession(n, session);
                }
            }
        }

        public static Session AddSession(int index, IVirtualSession session)
        {
            session.Index = index;
            Sessions[index].Add(session);
            VirtualSessionCreated?.Invoke(null, new SessionArgs(index, Sessions[index].IndexOf(session), session));
            return new Session
            {
                Container = session.Save(),
                UUID = session.UUID,
                Key = session.Key
            };
        }

        public static IVirtualSession AddSession(int index, Session session)
        {
            if (string.IsNullOrEmpty(session.Key) || !Creators.ContainsKey(session.Key))
                return null;
            var creator = Creators[session.Key];
            var virtualSession = (IVirtualSession) null;
            try
            {
                virtualSession = creator.CreateSession(index, session.Container, session.UUID);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }

            virtualSession.Index = index;
            Sessions[index].Add(virtualSession);
            VirtualSessionCreated?.Invoke(null, new SessionArgs(index, Sessions[index].IndexOf(virtualSession), virtualSession));
            return virtualSession;
        }

        public static void RemoveSession(int index, int internalIndex)
        {
            var session = Sessions[index][internalIndex];
            session.Dispose();
            VirtualSessionRemoved?.Invoke(null, new SessionArgs(index, internalIndex, session));
            Sessions[index].RemoveAt(internalIndex);
            
        }
        
        public static void RemoveSession(int index, IVirtualSession session)
        {
            session.Dispose();
            VirtualSessionRemoved?.Invoke(null, new SessionArgs(index, Sessions[index].IndexOf(session), session));
            Sessions[index].Remove(session);
        }
        
        public static void RegisterCreator(string key, IVirtualSessionCreator creator1)
        {
            if (Creators.ContainsKey(key))
                return;
            Creators.Add(key, creator1);
        }
        
        public static void UnregisterCreator(string key)
        {
            if (Creators.ContainsKey(key))
                Creators.Remove(key);
        }

        /// <summary>
        /// Sets volume of all sliders in specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="volume"></param>
        /// <param name="selfInvoke"></param>
        public static void SetVolume(int index, float volume, bool selfInvoke)
        {
            try
            {
                if (index >= Sessions.Count)
                    return;
                foreach (var slider in Sessions[index])
                {
                    VolumeThread.BeginInvoke(() => { slider.Volume = volume; });
                }
            }
            finally
            {
                SessionVolumeChanged?.Invoke(null, new VolumeChangedArgs(volume, selfInvoke, index));
            }
        }
        
        /// <summary>
        /// Sets mute of all sliders in specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mute"></param>
        /// <param name="selfInvoke"></param>
        public static void SetMute(int index, bool mute, bool selfInvoke)
        {
            try
            {
                if (index >= Sessions.Count)
                    return;
                foreach (var slider in Sessions[index])
                {
                    VolumeThread.BeginInvoke(() => { slider.IsMute = mute; });
                }
            }
            finally
            {
                SessionMuteChanged?.Invoke(null, new MuteChangedArgs(mute, selfInvoke, index));
            }
        }
        
        /// <summary>
        /// Checks whatever is there any active slider in specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool HasActiveSession(int index)
        {
            if (index >= Sessions.Count)
                return false;
            if (Sessions[index].Count == 0)
                return false;
            return Sessions[index].Any(slider => slider.State == SessionState.ACTIVE);
        }
    
        /// <summary>
        /// Gets all session from devices.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IAudioSession> GetAllSessions()
        {
            return SessionController.Values.SelectMany(sessionController => sessionController.All());
        }
        
        public static IEnumerable<IAudioSession> GetSessions(string id)
        {
            return GetAllSessions().Where(x => x.Id == id);
        }
        
        /// <summary>
        /// Gets cached controller whatever possible.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static IAudioSessionController GetController(IDevice device)
        {
            var deviceId = device.Id;
            if (SessionController.TryGetValue(deviceId, out var controller))
                return controller;
            return device.GetCapability<IAudioSessionController>();
        }

        public static string GetSessionExec(IAudioSession session)
        {
            var processId = session.ProcessId;
            if (!_execPaths.ContainsKey(processId))
                return string.Empty;
            return _execPaths[processId];
        }
        
        #endregion
        
        #region Private Events

        private static void DeviceAdded(IDevice device)
        {
            try
            {
                var deviceId = device.Id;
                if (SessionController.ContainsKey(deviceId))
                    return;
                var controller = device.GetCapability<IAudioSessionController>();
                if (controller == default)
                    return;
                foreach (var session in controller.All())
                    AttachProcessExit(session);
                controller.SessionCreated.Subscribe(AttachProcessExit);
                SessionController.Add(deviceId, controller);
            }
            catch (Exception exception)
            {
                //critical for debugging
                Logger.Warn(exception);
            }
            finally
            {
                DeviceAddedCallback?.Invoke(device);
            }
        }
        
        private static void DeviceRemove(IDevice device)
        {
            try
            {
                var deviceId = device.Id;
                if (!SessionController.ContainsKey(deviceId))
                    return;
                var controller = SessionController[deviceId];
                foreach (var session in controller.All())
                    DetachProcessExit(session);
                SessionController.Remove(deviceId);
            }
            catch (Exception exception)
            {
                Logger.Warn(exception);
            }
            finally
            {
                DeviceRemovedCallback?.Invoke(device);
            }
        }

        private static void AttachProcessExit(IAudioSession session)
        {
            if(session.IsSystemSession) return;
            
            var processId = session.ProcessId;
            var execPath = ProcessWrapper.GetFileName(session.ProcessId);
            _execPaths.Set(processId, execPath);
            
            SessionCreated?.Invoke(session);
            _processWatcher.AttachProcessWait(session.ProcessId, id => SessionExited?.Invoke(session));
        }
        
        private static void DetachProcessExit(IAudioSession session){
            if(session.IsSystemSession) return;
            SessionExited?.Invoke(session);
            _execPaths.Remove(session.ProcessId);
            _processWatcher.DetachProcessWait(session.ProcessId);
        }

        #endregion
    }

    public class SessionArgs : EventArgs
    {
        public int Index { get; set; }
        public int SessionIndex { get; set; }
        public IVirtualSession Session { get; set; }

        public SessionArgs(int index, int sessionIndex, IVirtualSession session)
        {
            Index = index;
            SessionIndex = sessionIndex;
            Session = session;
        }
    }
    
    public class VolumeChangedArgs : EventArgs
    {
        public float Volume { get; set; }
        public bool SelfInvoke { get; set; }
        public int Index { get; set; }

        public VolumeChangedArgs(float volume, bool selfInvoke, int index)
        {
            Volume = volume;
            SelfInvoke = selfInvoke;
            Index = index;
        }
    }

    public class MuteChangedArgs : EventArgs
    {
        public bool Mute { get; set; }
        public bool SelfInvoke { get; set; }
        public int Index { get; set; }

        public MuteChangedArgs(bool mute, bool selfInvoke, int index)
        {
            Mute = mute;
            SelfInvoke = selfInvoke;
            Index = index;
        }
    }
}