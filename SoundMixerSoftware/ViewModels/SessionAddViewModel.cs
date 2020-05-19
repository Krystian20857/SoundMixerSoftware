using System;
using System.Diagnostics;
using Caliburn.Micro;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Models;
using System.Linq;
using System.Windows;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using SoundMixerSoftware.Common.Extension;
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

        private BindableCollection<SessionModel> _sessions = new BindableCollection<SessionModel>();
        private BindableCollection<AudioDeviceModel> _defaultDevices = new BindableCollection<AudioDeviceModel>();
        private BindableCollection<AudioDeviceModel> _devices = new BindableCollection<AudioDeviceModel>();

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
        public BindableCollection<AudioDeviceModel> DefaultDevices
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
        public SessionModel SelectedApp { get; set; }
        /// <summary>
        /// Currently Selected Default Audio Device
        /// </summary>
        public AudioDeviceModel SelectedDefaultDevice { get; set; }
        /// <summary>
        /// Currently Selected Audio Device
        /// </summary>
        public AudioDeviceModel SelectedDevice { get; set; }

        #endregion

        #region Constructor

        public SessionAddViewModel()
        {
            _sessionEnumerator = new SessionEnumerator(_deviceEnumerator.DefaultOutput);

            CreateDefault();

            foreach (var device in _deviceEnumerator.AllDevices)
            {
                Devices.Add(new AudioDeviceModel
                {
                    Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource(),
                    Name = device.FriendlyName,
                    Id = device.ID,
                });
            }

            foreach (var session in _sessionEnumerator.AudioProcesses)
            {
                var process = session.process;
                AddSession(process);
            }

            _sessionEnumerator.SessionCreated += SessionEnumeratorOnSessionCreated;
            _sessionEnumerator.StateChanged += SessionEnumeratorOnStateChanged;

            _deviceEnumerator.DefaultDeviceChange += DeviceEnumeratorOnDefaultDeviceChange;
        }

        #endregion

        #region Private Events
        
        private void DeviceEnumeratorOnDefaultDeviceChange(object sender, DefaultDeviceChangedArgs e)
        {
            CreateDefault();
        }
        
        private void SessionEnumeratorOnStateChanged(object sender, AudioSessionState e)
        {
            if (e != AudioSessionState.AudioSessionStateExpired)
                return;
            var sessionControl = sender as AudioSessionControl;
            Sessions.Remove(Sessions.First(x => x.ProcessID == sessionControl.GetProcessID));
        }

        private void SessionEnumeratorOnSessionCreated(object sender, AudioSessionControl e)
        {
            var process = Process.GetProcessById((int) e.GetProcessID);
            AddSession(process);
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
                DefaultDevices.Add(new AudioDeviceModel
                {
                    Image = IconExtractor.ExtractFromIndex(_deviceEnumerator.DefaultOutput.IconPath).ToImageSource(),
                    Name = _deviceEnumerator.DefaultOutput.FriendlyName
                });

                DefaultDevices.Add(new AudioDeviceModel
                {
                    Image = IconExtractor.ExtractFromIndex(_deviceEnumerator.DefaultInput.IconPath).ToImageSource(),
                    Name = _deviceEnumerator.DefaultInput.FriendlyName
                });
            });
        }

        /// <summary>
        /// Add audio session to Sessions tab.
        /// </summary>
        /// <param name="process"></param>
        private void AddSession(Process process)
        {
            Execute.OnUIThread(() =>
            {
                Sessions.Add(new SessionModel()
                {
                    Name = process.ProcessName,
                    Image = process.GetIcon().ToImageSource(),
                    ProcessID = process.Id
                });
            });
        }
        
        public void DefaultDevicesSelectionChanged()
        {
            SelectedApp = null;
            SelectedDevice = null;
        }

        public void DevicesSelectionChanged()
        {
            SelectedApp = null;
            SelectedDefaultDevice = null;
        }

        public void SessionsSelectionChanged()
        {
            SelectedDevice = null;
            SelectedDefaultDevice = null;
        }

        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        public void AddClick()
        {
            TryClose();
        }

        #endregion
    }
}