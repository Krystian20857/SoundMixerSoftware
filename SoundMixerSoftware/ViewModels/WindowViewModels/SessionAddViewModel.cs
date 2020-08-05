using System;
using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Models;
using System.Linq;
using NAudio.CoreAudioApi;
using NLog;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Helpers.Threading;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Win32.Wrapper;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of session Add Window.
    /// </summary>
    public class SessionAddViewModel : Screen
    {
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private SessionEnumerator _sessionEnumerator;
        private readonly DeviceEnumerator _deviceEnumerator = new DeviceEnumerator();
        private readonly int _sliderIndex;
        private SessionModel _selectedDevice;
        private BindableCollection<SessionModel> _deviceSessions = new BindableCollection<SessionModel>();

        private BindableCollection<SessionModel> _sessions = new BindableCollection<SessionModel>();
        private BindableCollection<SessionModel> _defaultDevices = new BindableCollection<SessionModel>();
        private BindableCollection<SessionModel> _devices = new BindableCollection<SessionModel>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Audio session collection.
        /// </summary>
        public BindableCollection<SessionModel> Sessions
        {
            get => _sessions;
            set => _sessions = value;
        }

        /// <summary>
        /// Default devices connection.
        /// </summary>
        public BindableCollection<SessionModel> DefaultDevices
        {
            get => _defaultDevices;
            set => _defaultDevices = value;
        }

        /// <summary>
        /// Device collection
        /// </summary>
        public BindableCollection<SessionModel> Devices
        {
            get => _devices;
            set => _devices = value;
        }
        
        /// <summary>
        /// Currently Selected Audio Session
        /// </summary>
        public SessionModel SelectedSession { get; set; }// = new SessionModel();

        public BindableCollection<SessionModel> DeviceSessions
        {
            get => _deviceSessions;
            set => _deviceSessions = value;
        }

        public SessionModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                SetSessions(_selectedDevice.ID);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create session add view-model.
        /// </summary>
        /// <param name="sliderIndex"></param>
        public SessionAddViewModel(int sliderIndex)
        {
            _sliderIndex = sliderIndex;

            CreateDefault();

            foreach (var device in _deviceEnumerator.AllDevices)
            {
                AddDevice(device);
            }
            
            _deviceEnumerator.DefaultDeviceChange += DeviceEnumeratorOnDefaultDeviceChange;
            _deviceEnumerator.DeviceAdded += DeviceEnumeratorOnDeviceAdded;
            _deviceEnumerator.DeviceRemoved += DeviceEnumeratorOnDeviceRemoved;

            SelectedDevice = DeviceSessions.First(x => x.ID == _deviceEnumerator.DefaultMultimediaRenderID);
        }

        #endregion

        #region Private Events

        /// <summary>
        /// Occurs when default audio device has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceEnumeratorOnDefaultDeviceChange(object sender, DefaultDeviceChangedArgs e)
        {
            var sessionMode = e.Role == Role.Communications ? SessionMode.DEFAULT_COMMUNICATION : SessionMode.DEFAULT_MULTIMEDIA;
            Execute.BeginOnUIThread(() =>
            {
                var deviceToRemove = DefaultDevices.FirstOrDefault(x => x.DataFlow == e.DataFlow && x.SessionMode == sessionMode);
                if (deviceToRemove == default)
                    return;
                DefaultDevices.Remove(deviceToRemove);
                CreateDefaultDevice(sessionMode, e.DataFlow, sender as string);
            });
        }

        private void SessionEnumeratorOnSessionExited(object sender, string sessionId)
        {
            var sessionToRemove = Sessions.FirstOrDefault(x => x.ID == sessionId);
            if (sessionToRemove == default(SessionModel) || !Sessions.Contains(sessionToRemove))
                return;
            Sessions.Remove(sessionToRemove);
        }

        private void SessionEnumeratorOnSessionCreated(object sender, AudioSessionControl e)
        {
            Execute.OnUIThread(() => { AddSession(e); });
        }
        
        private void DeviceEnumeratorOnDeviceRemoved(object sender, EventArgs e)
        {
            var deviceId = sender as string;
            Devices.Remove(Devices.First(x => x.ID == deviceId));
            DeviceSessions.Remove(DeviceSessions.First(x => x.ID == deviceId));
        }

        private void DeviceEnumeratorOnDeviceAdded(object sender, EventArgs e)
        {
            Execute.OnUIThread(() => { AddDevice(_deviceEnumerator.GetDeviceById(sender as string)); });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fill default devices tab with default devices.
        /// </summary>
        private void CreateDefault()
        {
            Execute.OnUIThread(() =>
            {
                CreateDefaultDevice(SessionMode.DEFAULT_MULTIMEDIA, DataFlow.Render, _deviceEnumerator.DefaultMultimediaRenderID);
                CreateDefaultDevice(SessionMode.DEFAULT_MULTIMEDIA, DataFlow.Capture, _deviceEnumerator.DefaultMultimediaCaptureID);
                CreateDefaultDevice(SessionMode.DEFAULT_COMMUNICATION, DataFlow.Render, _deviceEnumerator.DefaultCommunicationRenderID);
                CreateDefaultDevice(SessionMode.DEFAULT_COMMUNICATION, DataFlow.Capture, _deviceEnumerator.DefaultCommunicationCaptureID);
            });
        }
        
        private void CreateDefaultDevice(SessionMode sessionMode, DataFlow dataFlow, string deviceId)
        {
            var name = string.Empty;
            switch (sessionMode)
            {
                case SessionMode.DEFAULT_MULTIMEDIA:
                    switch (dataFlow)
                    {
                        case DataFlow.Render:
                            name = "Default Speaker";
                            break;
                        case DataFlow.Capture:
                            name = "Default Microphone";
                            break;
                    }
                    break;
                case SessionMode.DEFAULT_COMMUNICATION:
                    switch (dataFlow)
                    {
                        case DataFlow.Render:
                            name = "Default Communication Speaker";
                            break;
                        case DataFlow.Capture:
                            name = "Default Communication Microphone";
                            break;
                    }
                    break;
            }
            try
            {
                var device = _deviceEnumerator.GetDeviceById(deviceId);
                DefaultDevices.Add(new SessionModel
                {
                    Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                    Name = $"{name}({device.FriendlyName})",
                    SessionMode = sessionMode,
                    ID = sessionMode.CreateUUIDString(dataFlow),
                    DataFlow = dataFlow
                });
            }
            catch (Exception ex)
            {
                Logger.Warn("Error while creating default device for view:");
                Logger.Warn(ex);
            }
        }

        private void SetSessions(string deviceId)
        {
            if (_sessionEnumerator != null)
            {
                _sessionEnumerator.SessionCreated -= SessionEnumeratorOnSessionCreated;
                _sessionEnumerator.SessionExited -= SessionEnumeratorOnSessionExited;
            }

            Sessions.Clear();
            _sessionEnumerator = new SessionEnumerator(_deviceEnumerator.GetDeviceById(deviceId), ProcessWatcher.DefaultProcessWatcher);
                
            var sessions = _sessionEnumerator.AudioSessions;
            for (var n = 0; n < sessions.Count; n++)
            {
                var session = sessions[n];
                if(!session.IsSystemSoundsSession)
                    AddSession(session);
            }

            _sessionEnumerator.SessionCreated += SessionEnumeratorOnSessionCreated;
            _sessionEnumerator.SessionExited += SessionEnumeratorOnSessionExited;
        }

        private void AddDevice(MMDevice device)
        {
            Devices.Add(new SessionModel
            {
                Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                Name = device.FriendlyName,
                ID = device.ID,
                SessionMode = SessionMode.DEVICE,
                DataFlow = device.DataFlow
            });
            DeviceSessions.Add(new SessionModel
            {
                Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                Name = $"{device.FriendlyName} - ({(device.DataFlow == DataFlow.Capture ? "Input" : "Output")})",
                ID = device.ID,
                SessionMode = SessionMode.DEVICE,
                DataFlow = device.DataFlow
            });
        }

        /// <summary>
        /// Add audio session to Sessions tab.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="session"></param>
        private void AddSession(AudioSessionControl session)
        {
            if (!ProcessUtils.IsAlive((int) session.GetProcessID))
                return;
            var process = Process.GetProcessById((int) session.GetProcessID);

            Sessions.Add(new SessionModel()
            {
                Name = process.GetPreciseName(),
                Image = (process.GetMainWindowIcon() ?? ExtractedIcons.FailedIcon).ToImageSource(),
                ID = session.GetSessionIdentifier,
            });
        }

        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        public void AddClick()
        {
            if (SelectedSession == null)
                return;
            var profile = ProfileHandler.SelectedProfile;

            if(profile.Sliders == null)
                profile.Sliders = new List<SliderStruct>();
            if(profile.Sliders.Count <= _sliderIndex)
                for(var n = profile.Sliders.Count; n < _sliderIndex + 1; n++)
                    profile.Sliders.Add(new SliderStruct());

            try
            {
                var slider = profile.Sliders[_sliderIndex];
                if (slider.Applications.All(x => x.ID != SelectedSession.ID))
                {
                    var session = new Session
                    {
                        SessionMode = SelectedSession.SessionMode,
                        DataFlow = SelectedSession.DataFlow,
                        Name = SelectedSession.Name,
                        ID = SelectedSession.ID
                    };
                    SessionHandler.AddSlider(_sliderIndex, session);
                    slider.Applications.Add(session);
                    ProfileHandler.ProfileManager.SaveAll();
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler.HandleException(Logger, exception.Message,exception);
            }

            TryCloseAsync();
        }

        #endregion
    }
}