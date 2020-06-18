using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.AudioLib.SliderLib;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Helpers.Profile;

namespace SoundMixerSoftware.Helpers.AudioSessions
{
    public static class SessionHandler
    {
        #region Private Fields

        #endregion

        #region Public Fields

        public static DeviceEnumerator DeviceEnumerator { get; private set; } = new DeviceEnumerator();
        public static Dictionary<string, SessionEnumerator> SessionEnumerators { get; } = new Dictionary<string, SessionEnumerator>();
        public static List<List<IVirtualSlider>> Sliders { get; } = new List<List<IVirtualSlider>>();
        public static List<List<string>> RequestedSliders { get; } = new List<List<string>>();
        private static DebounceDispatcher _debounceDispatcher = new DebounceDispatcher();

        #endregion

        #region Events

        public static event EventHandler<SliderAddedArgs> SessionAdded;
        public static event EventHandler<SessionActiveArgs> SessionActive;
        public static event EventHandler<SliderAddedArgs> SessionDisconnected;
        public static event EventHandler<VolumeChangedArgs> VolumeChange; 
        public static event EventHandler<MuteChangedArgs> MuteChanged;

        #endregion

        #region Constructor

        static SessionHandler()
        {
            ReloadSessionHandler();
        }

        public static void ReloadSessionHandler()
        {
            foreach (var sessionEnumerator in SessionEnumerators)
                sessionEnumerator.Value.Dispose();
            SessionEnumerators.Clear();
            
            DeviceEnumerator.Dispose();
            DeviceEnumerator = new DeviceEnumerator();
            
            foreach (var device in DeviceEnumerator.OutputDevices)
            {
                var sessionEnum = new SessionEnumerator(device);
                sessionEnum.SessionCreated += SessionEnumeratorOnSessionCreated;
                sessionEnum.SessionExited += SessionEnumeratorOnSessionExited;
                SessionEnumerators.Add(device.ID, sessionEnum);
            }
            
            SessionAdded += OnSessionAdded;
        }

        private static void SessionEnumeratorOnSessionExited(object sender, string e)
        {
            var sessionControl = sender as AudioSessionControl;
            for (var n = 0; n < Sliders.Count; n++)
            {
                var sliders = Sliders[n];
                for (var x = 0; x < sliders.Count; x++)
                {
                    var slidertmp = sliders[x];
                    if (!(slidertmp is SessionSlider slider)) continue;
                    if (slider.SessionID == sessionControl.GetSessionIdentifier)
                    {
                        var name = ProfileHandler.SelectedProfile.Sliders[n].Applications[x].Name;
                        sliders.RemoveAt(x);
                        SessionDisconnected?.Invoke(null, new SliderAddedArgs(slider, new Session
                        {
                            ID = sessionControl.GetSessionIdentifier,
                            Name = name,
                            SessionMode = SessionMode.Session
                        }, n, SessionState.Disconnected));
                        RequestedSliders[n].Add(sessionControl.GetSessionIdentifier);
                    }
                }
            }
        }

        #endregion
        
        #region Private Events

        private static void OnSessionAdded(object sender, SliderAddedArgs e)
        {
            if(e.SessionState == SessionState.Disconnected || e.SessionState == SessionState.DeviceNotDetected)
                RequestedSliders[e.Index].Add(e.Session.ID);
        }

        private static void SessionEnumeratorOnSessionCreated(object sender, AudioSessionControl e)
        {
            for (var n = 0; n < RequestedSliders.Count; n++)
            {
                var ids = RequestedSliders[n];
                for (var x = 0; x < ids.Count; x++)
                {
                    var id = ids[x];
                    if (id.Equals(e.GetSessionIdentifier, StringComparison.InvariantCultureIgnoreCase))
                    {
                        RequestedSliders[n].RemoveAt(x);

                        var session = AddSlider(n, e);
                        if(session != null)
                            SessionActive?.Invoke(null, new SessionActiveArgs(session, n, e));
                    }
                }
            }
            
        }
        
        #endregion
        
        #region Public Methods

        public static void SetVolume(int index, float volume, bool selfInvoke)
        {
            if (index >= Sliders.Count)
                return;
            var sliders = Sliders[index];
            for (var n = 0; n < sliders.Count; n++)
                sliders[n].Volume = volume;
            VolumeChange?.Invoke(null, new VolumeChangedArgs(volume, selfInvoke, index));
        }

        public static void SetMute(int index, bool mute, bool selfInvoke)
        {
            if (index >= Sliders.Count)
                return;
            var sliders = Sliders[index];
            for (var n = 0; n < sliders.Count; n++)
                sliders[n].IsMute = mute;

            MuteChanged?.Invoke(null, new MuteChangedArgs(mute, selfInvoke, index));
        }

        public static void CreateSliders()
        {
            Sliders.Clear();
            RequestedSliders.Clear();
            for (var n = 0; n < ProfileHandler.SelectedProfile.SliderCount; n++)
            {
                Sliders.Add(new List<IVirtualSlider>());
                RequestedSliders.Add(new List<string>());
            }

            foreach (var sliderStruct in ProfileHandler.SelectedProfile.Sliders)
            {
                foreach (var session in sliderStruct.Applications)
                {
                    AddSlider(sliderStruct.Index, session);
                }
            }
        }

        public static bool AddSlider(int index, Session session)
        {
            if (Sliders.Count <= index)
                return false;
            IVirtualSlider slider = null;
            switch (session.SessionMode)
            {
                case SessionMode.Device:
                    try
                    {
                        slider = new DeviceSlider(DeviceEnumerator.GetDeviceById(session.ID));
                    }
                    catch
                    {
                        SessionAdded?.Invoke(null, new SliderAddedArgs(null, session, index, SessionState.Disconnected));
                        return true;
                    }
                    break;
                case SessionMode.Session:
                    //var audioSession = SessionEnumerator.GetById(session.ID);
                    var deviceID = Identifier.GetDeviceId(session.ID);
                    if (!SessionEnumerators.ContainsKey(deviceID))
                    {
                        SessionAdded?.Invoke(null, new SliderAddedArgs(null, session, index, SessionState.DeviceNotDetected));
                        return false;
                    }
                    var audioSession = SessionEnumerators[deviceID].GetById(session.ID);
                    if(audioSession != null)
                        slider = new SessionSlider(audioSession);
                    else
                    {
                        SessionAdded?.Invoke(null, new SliderAddedArgs(null, session, index, SessionState.Disconnected));
                        return true;
                    }
                    break;
                case SessionMode.DefaultInputDevice:
                    slider = new DefaultDeviceSlider(false);
                    break;
                case SessionMode.DefaultOutputDevice:
                    slider =  new DefaultDeviceSlider(true);
                    break;
            }
            Sliders[index].Add(slider);
            SessionAdded?.Invoke(null, new SliderAddedArgs(slider, session, index, SessionState.Active));
            return true;
        }

        public static Session AddSlider(int index, AudioSessionControl audioSession)
        {
            if (Sliders.Count <= index)
                return null;
            var slider = new SessionSlider(audioSession);
            var session = new Session()
            {
                DataFlow = DataFlow.All,
                ID = audioSession.GetSessionIdentifier,
                Name = Process.GetProcessById((int) audioSession.GetProcessID).ProcessName,
                SessionMode = SessionMode.Session
            };
            Sliders[index].Add(slider);
            return session;
        }
        
        public static void RemoveSlider(int index, Session session)
        {
            var sliders = Sliders[index];
            if (session.SessionMode == SessionMode.Device)
            {
                for (var n = 0; n < sliders.Count; n++)
                {
                    var slider = sliders[n];
                    if(slider is DeviceSlider deviceSlider)
                        if(deviceSlider.DeviceID == session.ID)
                            sliders.RemoveAt(n);
                }
            }
            else if (session.SessionMode == SessionMode.Session)
            {
                for (var n = 0; n < sliders.Count; n++)
                {
                    var slider = sliders[n];
                    if(slider is SessionSlider sessionSlider)
                        if(sessionSlider.SessionID == session.ID)
                            sliders.RemoveAt(n);
                }
            }
            else if (session.SessionMode == SessionMode.DefaultInputDevice)
            {
                for (var n = 0; n < sliders.Count; n++)
                {
                    var slider = sliders[n];
                    if(slider is DefaultDeviceSlider sessionSlider)
                        if(!sessionSlider.IsDefaultOutput)
                            sliders.RemoveAt(n);
                }
            }
            else if (session.SessionMode == SessionMode.DefaultOutputDevice)
            {
                for (var n = 0; n < sliders.Count; n++)
                {
                    var slider = sliders[n];
                    if(slider is DefaultDeviceSlider sessionSlider)
                        if(sessionSlider.IsDefaultOutput)
                            sliders.RemoveAt(n);
                }
            }
            
        }
        
        #endregion
    }

    public class SliderAddedArgs : EventArgs
    {
        public IVirtualSlider Slider { get; set; }
        public Session Session { get; set; }
        public int Index { get; set; }

        public SessionState SessionState { get; set; }

        public SliderAddedArgs(IVirtualSlider slider, Session session, int index, SessionState sessionState)
        {
            Slider = slider;
            Session = session;
            Index = index;
            SessionState = sessionState;
        }
    }

    public class SessionActiveArgs : EventArgs
    {
        public AudioSessionControl SessionControl { get; set; }
        public Session Session { get; set; }
        public int Index { get; set; }

        public SessionActiveArgs(Session session, int index, AudioSessionControl sessionControl)
        {
            Session = session;
            Index = index;
            SessionControl = sessionControl;
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