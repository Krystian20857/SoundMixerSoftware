using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Models;
using System.Linq;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of session Add Window.
    /// </summary>
    public class SessionAddViewModel : Screen
    {
        #region Private Fields

        private readonly SessionEnumerator _sessionEnumerator;
        private readonly DeviceEnumerator _deviceEnumerator = new DeviceEnumerator();
        private readonly int _sliderIndex;

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
        public SessionModel SelectedSession { get; set; } = new SessionModel();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Create session add view-model.
        /// </summary>
        /// <param name="sliderIndex"></param>
        public SessionAddViewModel(int sliderIndex)
        {
            _sliderIndex = sliderIndex;
            _sessionEnumerator = new SessionEnumerator(_deviceEnumerator.DefaultOutput);
            CreateDefault();

            foreach (var device in _deviceEnumerator.AllDevices)
            {
                Devices.Add(new SessionModel
                {
                    Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                    Name = device.FriendlyName,
                    ID = device.ID,
                    SessionMode = SessionMode.Device,
                    DataFlow = device.DataFlow
                });
            }

            var sessions = _sessionEnumerator.AudioSessions;
            for (var n = 0; n < sessions.Count; n++)
            {
                var session = sessions[n];
                if(!session.IsSystemSoundsSession)
                    AddSession(session);
            }

            _sessionEnumerator.SessionCreated += SessionEnumeratorOnSessionCreated;
            _sessionEnumerator.SessionExited += SessionEnumeratorOnSessionExited;

            _deviceEnumerator.DefaultDeviceChange += DeviceEnumeratorOnDefaultDeviceChange;
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
            CreateDefault();
        }
        
        private void SessionEnumeratorOnSessionExited(object sender, string e)
        {
            var audioSession = sender as AudioSessionControl;
            Sessions.Where(x => x.ID == audioSession.GetSessionIdentifier).ToList().ForEach(y =>
            {
                Sessions.Remove(y);
            });
        }

        private void SessionEnumeratorOnSessionCreated(object sender, AudioSessionControl e)
        {
            Execute.OnUIThread(() => { AddSession(e); });
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
                DefaultDevices.Clear();
                DefaultDevices.Add(new SessionModel
                {
                    Image = IconExtractor.ExtractFromIndex(_deviceEnumerator.DefaultOutput.IconPath).ToImageSource(),
                    Name = _deviceEnumerator.DefaultOutput.FriendlyName,
                    SessionMode = SessionMode.DefaultOutputDevice,
                    ID = _deviceEnumerator.DefaultOutput.ID,
                    DataFlow = DataFlow.Render
                });

                DefaultDevices.Add(new SessionModel
                {
                    Image = IconExtractor.ExtractFromIndex(_deviceEnumerator.DefaultInput.IconPath).ToImageSource(),
                    Name = _deviceEnumerator.DefaultInput.FriendlyName,
                    SessionMode = SessionMode.DefaultInputDevice,
                    ID = _deviceEnumerator.DefaultInput.ID,
                    DataFlow = DataFlow.Capture
                });
            });
        }

        /// <summary>
        /// Add audio session to Sessions tab.
        /// </summary>
        /// <param name="process"></param>
        private void AddSession(AudioSessionControl session)
        {
            if (Process.GetProcesses().All(x => x.Id != (int) session.GetProcessID))
                return;
            var process = Process.GetProcessById((int) session.GetProcessID);


                Sessions.Add(new SessionModel()
                {
                    Name = process.ProcessName,
                    Image = process.GetIcon().ToImageSource(),
                    ID = session.GetSessionIdentifier,
                });
        }
        
        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        public void AddClick()
        {
            var profile = ProfileHandler.SelectedProfile;

            if(profile.Sliders == null)
                profile.Sliders = new List<SliderStruct>();
            if(profile.Sliders.Count <= _sliderIndex)
                profile.Sliders.Add(new SliderStruct());

            var slider = profile.Sliders[_sliderIndex];
            if (slider.Applications.All(x => x.ID != SelectedSession.ID))
            {
                slider.Index = _sliderIndex;
                var session = new Session
                {
                    SessionMode = SelectedSession.SessionMode,
                    DataFlow = SelectedSession.DataFlow,
                    Name = SelectedSession.Name,
                    ID = SelectedSession.ID
                };
                SessionHandler.AddSlider(_sliderIndex,session);
                slider.Applications.Add(session);
                ProfileHandler.ProfileManager.SaveAll();
            }

            TryClose();
        }

        #endregion
    }
}