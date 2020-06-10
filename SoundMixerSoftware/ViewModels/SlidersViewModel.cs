using System;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Overlay.Resource;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// Device model of Sliders Tab.
    /// </summary>
    public class SlidersViewModel : PropertyChangedBase, ITabModel
    {
        #region Private Fields
        
        private BindableCollection<SliderModel> _sliders = new BindableCollection<SliderModel>();
        private IWindowManager _windowManager = new WindowManager();
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// All available sliders.
        /// </summary>
        public BindableCollection<SliderModel> Sliders
        {
            get => _sliders;
            set
            {
                _sliders = value;
                NotifyOfPropertyChange(() => Sliders);
            }
        }
        
        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion

        #region Constructor
        
        public SlidersViewModel()
        {
            Name = "Sliders";
            Icon = PackIconKind.VolumeSource;

            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;

            if(ProfileHandler.SelectedProfile == null)
                return;
            
            UpdateProfile();
            ThemeManager.Initialize();
        }

        #endregion

        #region Private Events

        private void UpdateProfile()
        {
            SessionHandler.SessionAdded -= SessionHandlerOnSessionAdded;
            SessionHandler.SessionActive -= SessionHandlerOnSessionActive;
            SessionHandler.SessionDisconnected -= SessionHandlerOnSessionDisconnected;
            SessionHandler.ClearAll -= SessionHandlerOnClearAll;

            ProfileHandler.ProfileManager.Load(ProfileHandler.SelectedGuid);
            Sliders.Clear();
            for (var n = 0; n < ProfileHandler.SelectedProfile.SliderCount; n++)
                Sliders.Add(new SliderModel
                {
                    Index = n
                });
            
            SessionHandler.SessionAdded += SessionHandlerOnSessionAdded;
            SessionHandler.SessionActive += SessionHandlerOnSessionActive;
            SessionHandler.SessionDisconnected += SessionHandlerOnSessionDisconnected;
            SessionHandler.ClearAll += SessionHandlerOnClearAll;
            SessionHandler.CreateSliders();
        }

        private void SessionHandlerOnClearAll(object sender, EventArgs e)
        {
            Execute.OnUIThread(() =>
            {
                Sliders.Clear();
                SessionHandler.CreateSliders();
            });
        }

        private void SessionHandlerOnSessionDisconnected(object sender, SliderAddedArgs e)
        {

            var apps = Sliders[e.Index].Applications;
            for (var n = 0; n < apps.Count; n++)
            {
                var app = apps[n];
                if (app.ID.Equals(e.Session.ID, StringComparison.InvariantCultureIgnoreCase))
                {
                    var n1 = n;
                    Execute.OnUIThread(() =>
                    {
                        apps.RemoveAt(n1);
                        apps.Add(TranslateModel(e));
                    });
                }
            }

        }

        private void SessionHandlerOnSessionActive(object sender, SessionActiveArgs e)
        {
            Execute.OnUIThread(() =>
            {
                var apps = Sliders[e.Index].Applications;
                for (var n = 0; n < apps.Count; n++)
                {
                    var app = apps[n];
                    if (!app.IsActive && app.ID.Equals(e.Session.ID, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var device = SessionHandler.DeviceEnumerator.GetDeviceById(Identifier.GetDeviceId(e.Session.ID));
                        apps.RemoveAt(n);
                        apps.Add(new SessionModel
                        {
                            ID = e.Session.ID,
                            Image = System.Drawing.Icon.ExtractAssociatedIcon(Process.GetProcessById((int) e.SessionControl.GetProcessID).GetFileName()).ToImageSource(),
                            IsActive = true,
                            Name = $"{e.Session.Name} - {device.FriendlyName}",
                            SessionMode = SessionMode.Session
                        });
                    }
                }
            });
        }

        private void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            UpdateProfile();
        }
        
        private void SessionHandlerOnSessionAdded(object sender, SliderAddedArgs e)
        {
            Execute.OnUIThread(() =>{
                Sliders[e.Index].Applications.Add(TranslateModel(e));
            });
        }

        private SessionModel TranslateModel(SliderAddedArgs e)
        {
            var session = e.Session;
            var model = new SessionModel
            {
                DataFlow = session.DataFlow,
                ID = session.ID,
                IsActive = e.IsActive,
                SessionMode = session.SessionMode
            };

            if(e.IsActive)
            {
                if (session.SessionMode == SessionMode.Device)
                {
                    var device = SessionHandler.DeviceEnumerator.GetDeviceById(session.ID);
                    model.Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource();
                    model.Name = device.FriendlyName;
                }
                else if (session.SessionMode == SessionMode.Session)
                {
                    var deviceID = Identifier.GetDeviceId(session.ID);
                    if (!SessionHandler.SessionEnumerators.ContainsKey(deviceID))
                    {
                        model.Image = ExtractedIcons.FailedIcon.ToImageSource();
                        model.Name = $"{session.Name}(Device not available)";
                    }
                    var pid = (int)SessionHandler.SessionEnumerators[deviceID].GetById(session.ID).GetProcessID;
                    var process = Process.GetProcessById(pid);
                    var device = SessionHandler.DeviceEnumerator.GetDeviceById(deviceID);
                    model.Image = System.Drawing.Icon.ExtractAssociatedIcon(process.GetFileName()).ToImageSource();
                    model.Name = $"{process.ProcessName} - {device.FriendlyName}";
                }
                else if (session.SessionMode == SessionMode.DefaultInputDevice)
                {
                    model.Name = "Default Microphone";
                    model.Image = ExtractedIcons.MicIcon.ToImageSource();
                }
                else if (session.SessionMode == SessionMode.DefaultOutputDevice)
                {
                    model.Name = "Default Speaker";
                    model.Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                }
            }
            else
            {
                var device = SessionHandler.DeviceEnumerator.GetDeviceById(Identifier.GetDeviceId(session.ID));
                model.Image = ExtractedIcons.FailedIcon.ToImageSource();
                model.Name = $"{session.Name} - {device.FriendlyName}(Not Active)";
            }

            return model;
        }

        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void AddClick(object sender)
        {
            var model = sender as SliderModel;
            var addViewModel = new SessionAddViewModel(model.Index);
            _windowManager.ShowDialog(addViewModel);
        }

        public void ReloadClick()
        {
            UpdateProfile();
        }

        /// <summary>
        /// Occurs when Remove Button has Clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void RemoveClick(object sender)
        {
            RemoveSession(sender as SliderModel);
        }

        private void RemoveSession(SliderModel model)
        {
            var session = model.SelectedApp;
            if (session == null)
                return;
            SessionHandler.RemoveSlider(model.Index, new Session()
            {
                SessionMode = session.SessionMode,
                DataFlow = session.DataFlow,
                ID = session.ID,
                Name = session.Name
            });
            var apps = ProfileHandler.SelectedProfile.Sliders[model.Index].Applications;
            if(session.SessionMode == SessionMode.Session || session.SessionMode == SessionMode.Device)
                apps.Remove(apps.First(x=> x.ID == session.ID));
            else if (session.SessionMode == SessionMode.DefaultInputDevice)
                apps.Remove(apps.First(x => x.SessionMode == SessionMode.DefaultInputDevice));
            else if (session.SessionMode == SessionMode.DefaultOutputDevice)
                apps.Remove(apps.First(x => x.SessionMode == SessionMode.DefaultOutputDevice));
            ProfileHandler.ProfileManager.Save(ProfileHandler.SelectedGuid);
            model.Applications.Remove(session);
        }

        /// <summary>
        /// Occurs when Mute Button has Clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void MuteClick(object sender)
        {
            var model = sender as SliderModel;
            model.Mute = !model.Mute;
        }
        
        #endregion
    }
}