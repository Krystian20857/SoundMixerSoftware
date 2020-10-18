using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using Caliburn.Micro;
using NLog;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Framework.AudioSessions;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Win32.Wrapper;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of session Add Window.
    /// </summary>z
    public class SessionAddViewModel : Screen
    {
        //#region Const

        //public static readonly Guid PROCESS_SESSIONS_UUID = new Guid("2BB69845-6FD9-4E26-A9CC-4EE031CC870A");
        
        //#endregion 
        
        #region Logger

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private IAudioSessionController _sessionController;
        private IAudioController _controller => SessionHandler.AudioController;

        private IDisposable _sessionCreateCallback;

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

        public BindableCollection<ProcessSessionModel> ProcessSessions { get; set; } = new BindableCollection<ProcessSessionModel>();

        public ISessionModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                SetSessions(_selectedDevice.Guid);
            }
        }

        #endregion
        
        #region Private Properties

        #endregion

        #region Constructor

        /// <summary>
        /// Create session add view-model.
        /// </summary>
        public SessionAddViewModel()
        {
            foreach (var device in _controller.GetDevices(DeviceState.Active))
                AddDevice(device);

            SessionHandler.DeviceAddedCallback += AddDevice;
            SessionHandler.DeviceRemovedCallback += RemoveDevice;

            SessionHandler.SessionExited += RemoveSession;
            SessionHandler.SessionExited += RemoveProcessSession;
            
            SessionHandler.SessionCreated += AddProcessSession;

            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Playback, Role.Multimedia));
            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Capture, Role.Multimedia));
            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Playback, Role.Communications));
            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Capture, Role.Communications));

            SelectedDevice = DeviceSessions.FirstOrDefault(x => x.Guid == _controller.DefaultPlaybackDevice.Id) ?? DeviceSessions.FirstOrDefault();
            
            foreach(var session in SessionHandler.GetAllSessions())
                AddProcessSession(session);
        }
        
        #endregion

        #region Private Methods

        private void SetSessions(Guid selectedDeviceId)
        {
            var device = _controller.GetDevice(selectedDeviceId, DeviceState.Active);
            if(device == default)
                return;
            if(!device.HasCapability<IAudioSessionController>())
                return;
            _sessionController = SessionHandler.GetController(device);
            _sessionCreateCallback?.Dispose();
            _sessionCreateCallback = _sessionController.SessionCreated.Subscribe(AddSession);
            
            Sessions.Clear();
            foreach (var session in _sessionController.All())
                AddSession(session);
        }

        private void AddDevice(IDevice device)
        {
            var deviceId = device.Id;
            if(Devices.Any(x => x.Guid == deviceId))
                return;
            var model = new AudioDeviceModel
            {
                Guid = deviceId,
                Name = device.FullName,
                Type = device.DeviceType
            };

            Execute.OnUIThread(() => model.Image = IconExtractor.ExtractFromIndex(device.IconPath)?.ToImageSource());

            if (device is CoreAudioDevice coreDevice)
                model.ID = coreDevice.RealId;

            Execute.OnUIThread(() => Devices.Add(model));
            
            if(device.IsPlaybackDevice)
                Execute.OnUIThread(() => DeviceSessions.Add(model));
        }

        private void AddSession(IAudioSession session)
        {
            if (Sessions.Any(x => x.ID == session.Id))
                return;
            
            var processId = session.ProcessId;
            if(!ProcessWrapper.IsAlive(processId))
                return;

            using (var process = Process.GetProcessById(processId))
            {
                var sessionModel = new AudioSessionModel
                {
                    ID = session.Id,
                    Name = process.GetPreciseName()
                };
                Execute.OnUIThread(() =>
                {
                    sessionModel.Image = (process.GetMainWindowIcon() ?? ExtractedIcons.FailedIcon).ToImageSource();
                    Sessions.Add(sessionModel);
                });
            }
        }

        private void AddProcessSession(IAudioSession session)
        {
            var executablePath = SessionHandler.GetSessionExec(session);
            var processId = session.ProcessId;
            if(ProcessSessions.Any(x => x.ExecutablePath == executablePath))
                return;
            
            if(!ProcessWrapper.IsAlive(processId))
                return;
            
            using (var process = Process.GetProcessById(processId))
            {
                var rawName = process.GetPreciseName();
                var sessionModel = new ProcessSessionModel()
                {
                    ID = session.Id,
                    ExecutablePath = executablePath,
                    Name = $"{rawName} - {Path.GetFileName(executablePath)}",
                    RawName = rawName,
                };
                Execute.OnUIThread(() =>
                {
                    sessionModel.Image = (process.GetMainWindowIcon() ?? ExtractedIcons.FailedIcon).ToImageSource();
                    ProcessSessions.Add(sessionModel);
                });
            }
        }

        private void RemoveDevice(IDevice device)
        {
            var deviceId = device.Id;
            var deviceToRemove = Devices.FirstOrDefault(x => x.Guid == deviceId);
            if (deviceToRemove != null)
                Devices.Remove(deviceToRemove);

            deviceToRemove = DeviceSessions.FirstOrDefault(x => x.Guid == deviceId);
            if (deviceToRemove != null)
            {
                if (SelectedDevice?.Guid == deviceToRemove.Guid)
                    SetSessions(_controller.DefaultPlaybackDevice.Id);
                DeviceSessions.Remove(deviceToRemove);
            }
        }

        private void RemoveSession(IAudioSession session)
        {
            var sessionModel = Sessions.FirstOrDefault(x => x.ID == session.Id);
            if(sessionModel == default)
                return;
            Sessions.Remove(sessionModel);
        }

        private void RemoveProcessSession(IAudioSession session)
        {
            var executablePath = SessionHandler.GetSessionExec(session);
            var sessionModel = ProcessSessions.FirstOrDefault(x => x.ExecutablePath == executablePath);
            if(sessionModel == default)
                return;
            ProcessSessions.Remove(sessionModel);
        }

        #endregion

        #region Private Events

        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        public void AddClick()
        {
            if (SelectedSession == null)
                return;
            SessionUtil.AddSession(SliderIndex, SelectedSession.CreateSession(SliderIndex));
            TryCloseAsync();
        }

        #endregion
    }
}