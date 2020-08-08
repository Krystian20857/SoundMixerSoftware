using System;
using System.Collections.Generic;
using System.Linq;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.Threading.Com;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Helpers.Threading;

namespace SoundMixerSoftware.Helpers.AudioSessions
{
    public static class SessionHandler
    {
        #region Private Fields
        
        #endregion
        
        #region Public Properties

        public static List<List<IVirtualSession>> Sessions { get; private set; } = new List<List<IVirtualSession>>();
        public static Dictionary<string, IVirtualSessionCreator> Creators { get; } = new Dictionary<string, IVirtualSessionCreator>();

        public static DeviceEnumerator DeviceEnumerator { get; private set; } = new DeviceEnumerator();
        public static Dictionary<string, SessionEnumerator> SessionEnumerators { get; private set; } = new Dictionary<string, SessionEnumerator>();

        #endregion
        
        #region Events

        public static event EventHandler<SessionArgs> SessionCreated;
        public static event EventHandler<SessionArgs> SessionRemoved;
        
        #endregion
        
        #region Constructor
        
        
        #endregion
        
        #region Public Methods
        
        public static void ReloadAll(){

            foreach(var sessionEnum in SessionEnumerators)
                sessionEnum.Value.Dispose();
            SessionEnumerators.Clear();
            
            DeviceEnumerator.Dispose();
            DeviceEnumerator = new DeviceEnumerator();

            foreach (var device in DeviceEnumerator.AllDevices)
            {
                var sessionEnum = new SessionEnumerator(device, ProcessWatcher.DefaultProcessWatcher);
                SessionEnumerators.Add(device.ID, sessionEnum);
            }

            DeviceEnumerator.DeviceAdded += DeviceEnumeratorOnDeviceAdded;
            DeviceEnumerator.DeviceRemoved += DeviceEnumeratorOnDeviceRemoved;
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
            Sessions[index].Add(session);
            SessionCreated?.Invoke(null, new SessionArgs(index, Sessions[index].IndexOf(session), session));
            return new Session
            {
                Container = session.Save(),
                UUID = session.UUID,
                Key = session.Key
            };
        }

        public static IVirtualSession AddSession(int index, Session session){
            if (!Creators.ContainsKey(session.Key) || string.IsNullOrEmpty(session.Key))
                return null;
            var creator = Creators[session.Key];
            var virtualSession = creator.CreateSession(index, session.Container, session.UUID);
            Sessions[index].Add(virtualSession);
            SessionCreated?.Invoke(null, new SessionArgs(index, Sessions[index].IndexOf(virtualSession), virtualSession));
            return virtualSession;
        }
        
        public static void RemoveSession(int index, int internalIndex)
        {
            var session = Sessions[index][internalIndex];
            SessionRemoved?.Invoke(null, new SessionArgs(index, internalIndex, session));
            Sessions[index].RemoveAt(internalIndex);
            
        }
        
        public static void RemoveSession(int index, IVirtualSession session)
        {
            SessionRemoved?.Invoke(null, new SessionArgs(index, Sessions[index].IndexOf(session), session));
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
            if (index >= Sessions.Count)
                return;
            foreach (var slider in Sessions[index])
            {
                slider.Volume = volume;
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
            if (index >= Sessions.Count)
                return;
            foreach (var slider in Sessions[index])
            {
                slider.IsMute = mute;
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
        
        #endregion
        
        #region Private Events

        private static void DeviceEnumeratorOnDeviceRemoved(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            if (!SessionEnumerators.ContainsKey(deviceId)) return;
            ComThread.Invoke(() => SessionEnumerators[deviceId].Dispose());
            SessionEnumerators.Remove(deviceId);
        }

        private static void DeviceEnumeratorOnDeviceAdded(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            if (SessionEnumerators.ContainsKey(deviceId)) return;
            ComThread.Invoke(() =>
            {
                var device =DeviceEnumerator.GetDeviceById(deviceId);
                 SessionEnumerators.Add(deviceId, new SessionEnumerator(device, ProcessWatcher.DefaultProcessWatcher));
            });
            
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