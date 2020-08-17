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
using SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Helpers.Threading;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Win32.Wrapper;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of session Add Window.
    /// </summary>z
    public class SessionAddViewModel : Screen
    {
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private SessionEnumerator _sessionEnumerator;
        private readonly DeviceEnumerator _deviceEnumerator = new DeviceEnumerator();
        private ISessionModel _selectedDevice;
        private BindableCollection<AudioDeviceModel> _deviceSessions = new BindableCollection<AudioDeviceModel>();

        private BindableCollection<AudioSessionModel> _sessions = new BindableCollection<AudioSessionModel>();
        private BindableCollection<DefaultDeviceModel> _defaultDevices = new BindableCollection<DefaultDeviceModel>();
        private BindableCollection<AudioDeviceModel> _devices = new BindableCollection<AudioDeviceModel>();

        #endregion

        #region Public Properties
        
        public static SessionAddViewModel Instance => IoC.Get<SessionAddViewModel>();

        public int SliderIndex { get; set; }

        /// <summary>
        /// Audio session collection.
        /// </summary>
        public BindableCollection<AudioSessionModel> Sessions
        {
            get => _sessions;
            set => _sessions = value;
        }

        /// <summary>
        /// Default devices connection.
        /// </summary>
        public BindableCollection<DefaultDeviceModel> DefaultDevices
        {
            get => _defaultDevices;
            set => _defaultDevices = value;
        }

        /// <summary>
        /// Device collection
        /// </summary>
        public BindableCollection<AudioDeviceModel> Devices
        {
            get => _devices;
            set => _devices = value;
        }
        
        /// <summary>
        /// Currently Selected Audio Session
        /// </summary>
        public ISessionModel SelectedSession { get; set; }// = new SessionModel();

        public BindableCollection<AudioDeviceModel> DeviceSessions
        {
            get => _deviceSessions;
            set => _deviceSessions = value;
        }

        public ISessionModel SelectedDevice
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
        public SessionAddViewModel()
        {
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
            var sessionMode = e.Role == Role.Communications ? DefaultDeviceMode.DEFAULT_COMMUNICATION : DefaultDeviceMode.DEFAULT_MULTIMEDIA;
            Execute.BeginOnUIThread(() =>
            {
                var deviceToRemove = DefaultDevices.FirstOrDefault(x => x.DataFlow == e.DataFlow && x.Mode == sessionMode);
                if (deviceToRemove == default)
                    return;
                DefaultDevices.Remove(deviceToRemove);
                CreateDefaultDevice(sessionMode, e.DataFlow, sender as string);
            });
        }

        private void SessionEnumeratorOnSessionExited(object sender, string sessionId)
        {
            var sessionToRemove = Sessions.FirstOrDefault(x => x.ID == sessionId);
            if (sessionToRemove == default || !Sessions.Contains(sessionToRemove))
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
                CreateDefaultDevice(DefaultDeviceMode.DEFAULT_MULTIMEDIA, DataFlow.Render, _deviceEnumerator.DefaultMultimediaRenderID);
                CreateDefaultDevice(DefaultDeviceMode.DEFAULT_MULTIMEDIA, DataFlow.Capture, _deviceEnumerator.DefaultMultimediaCaptureID);
                CreateDefaultDevice(DefaultDeviceMode.DEFAULT_COMMUNICATION, DataFlow.Render, _deviceEnumerator.DefaultCommunicationRenderID);
                CreateDefaultDevice(DefaultDeviceMode.DEFAULT_COMMUNICATION, DataFlow.Capture, _deviceEnumerator.DefaultCommunicationCaptureID);
            });
        }
        
        private void CreateDefaultDevice(DefaultDeviceMode sessionMode, DataFlow dataFlow, string deviceId)
        {
            var name = string.Empty;
            switch (sessionMode)
            {
                case DefaultDeviceMode.DEFAULT_MULTIMEDIA:
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
                case DefaultDeviceMode.DEFAULT_COMMUNICATION:
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
                DefaultDevices.Add(new DefaultDeviceModel()
                {
                    Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                    Name = $"{name}({device.FriendlyName})",
                    Mode = sessionMode,
                    ID = sessionMode.CreateStringUUID(dataFlow),
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
            if (Devices.Any(x => x.ID == device.ID))
                return;
            Devices.Add(new AudioDeviceModel()
            {
                Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                Name = device.FriendlyName,
                ID = device.ID,
                DataFlow = device.DataFlow
            });
            DeviceSessions.Add(new AudioDeviceModel
            {
                Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                Name = $"{device.FriendlyName} - ({(device.DataFlow == DataFlow.Capture ? "Input" : "Output")})",
                ID = device.ID,
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

            Sessions.Add(new AudioSessionModel
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
            SessionUtils.AddSession(SliderIndex, SelectedSession.CreateSession(SliderIndex));
            TryCloseAsync();
        }

        #endregion
    }
}