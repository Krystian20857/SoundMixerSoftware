using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using SoundMixerSoftware.Models;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using NLog;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions;
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

            SessionHandler.SessionExited += (session) => RemoveSession(session.Id);

            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Playback, Role.Multimedia));
            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Capture, Role.Multimedia));
            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Playback, Role.Communications));
            DefaultDevices.Add(new DefaultDeviceModel(DeviceType.Capture, Role.Communications));

            SelectedDevice = DeviceSessions.FirstOrDefault(x => x.Guid == _controller.DefaultPlaybackDevice.Id) ?? DeviceSessions.FirstOrDefault();
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
            var id = session.Id;
            if(Sessions.Any(x => x.ID == id))
                return;

            var processId = session.ProcessId;
            if(!ProcessUtils.IsAlive(processId))
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
        
        private void RemoveDevice(IDevice device)
        {
            var deviceId = device.Id;
            var deviceToRemove = Devices.FirstOrDefault(x => x.Guid == deviceId);
            if (deviceToRemove != null)
                Devices.Remove(deviceToRemove);
            
            deviceToRemove = DeviceSessions.FirstOrDefault(x => x.Guid == deviceId);
            if (deviceToRemove != null)
            {
                if(SelectedSession.Guid == deviceToRemove.Guid)
                    SetSessions(_controller.DefaultPlaybackDevice.Id);
                DeviceSessions.Remove(deviceToRemove);
            }
        }

        private void RemoveSession(string sessionId)
        {
            var sessionModel = Sessions.FirstOrDefault(x => x.ID == sessionId);
            if(sessionModel == default)
                return;
            Sessions.Remove(sessionModel);
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
            SessionUtils.AddSession(SliderIndex, SelectedSession.CreateSession(SliderIndex));
            TryCloseAsync();
        }

        #endregion
    }
}