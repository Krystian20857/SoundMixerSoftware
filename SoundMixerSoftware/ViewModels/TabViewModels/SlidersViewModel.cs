﻿using System;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.AudioLib;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Helpers.SliderConverter;
using SoundMixerSoftware.Helpers.SliderConverter.Converters;
using SoundMixerSoftware.Helpers.Utils;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Win32.Wrapper;
using ThemeManager = SoundMixerSoftware.Overlay.Resource.ThemeManager;

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
        public Guid Uuid { get; set; } = new Guid("5BABA481-CBAD-47B4-90EF-DD1B59E6694D");

        #endregion

        #region Constructor
        
        public SlidersViewModel()
        {
            ThemeManager.Initialize();
            Name = "Sliders";
            Icon = PackIconKind.VolumeSource;
            
            ConverterHandler.RegisterCreator("log_converter", new LogConverterCreator());

            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;

            if(ProfileHandler.SelectedProfile != null)
                UpdateProfile();
        }

        #endregion

        #region Private Events

        private void UpdateProfile()
        {
            SessionHandler.ReloadSessionHandler();
            SessionHandler.SessionAdded -= SessionHandlerOnSessionAdded;
            SessionHandler.SessionActive -= SessionHandlerOnSessionActive;
            SessionHandler.SessionDisconnected -= SessionHandlerOnSessionDisconnected;
            
            ConverterHandler.CreateConverters();
            
            foreach(var slider in Sliders)
                slider.Dispose();
            Sliders.Clear();
            
            var sliders = ProfileHandler.SelectedProfile.Sliders;
            var sliderCount = ProfileHandler.SelectedProfile.SliderCount;
            var modified = false;
            
            if (sliderCount >= sliders.Count)
            {
                for (var n = sliders.Count; n < sliderCount; n++)
                    sliders.Add(new SliderStruct {Name = $"Slider {n + 1}"});
                modified = true;
            }
            
            for (var n = 0; n < sliderCount; n++)
            {
                var slider = sliders[n];
                if (string.IsNullOrWhiteSpace(slider.Name))
                {
                    slider.Name = $"Slider {n + 1}";
                    modified = true;
                }

                var sliderModel = new SliderModel {Index = n, Name = slider.Name};
                var logScale = ConverterHandler.HasConverter<LogarithmicConverter>(n);
                if (logScale)
                    sliderModel.LogScale = true;
                Sliders.Add(sliderModel);
            }
            
            if(modified)
                ProfileHandler.SaveSelectedProfile();

            SessionHandler.SessionAdded += SessionHandlerOnSessionAdded;
            SessionHandler.SessionActive += SessionHandlerOnSessionActive;
            SessionHandler.SessionDisconnected += SessionHandlerOnSessionDisconnected;
            SessionHandler.CreateSliders();
        }

        private void SessionHandlerOnSessionDisconnected(object sender, SliderAddedArgs e)
        {
            var apps = Sliders[e.Index].Applications;
            for (var n = 0; n < apps.Count; n++)
            {
                var n1 = n;
                Execute.OnUIThread(() =>
                {
                    var app = apps[n1];
                    if (app.ID.Equals(e.Session.ID, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (apps.ElementAt(n1) != null)
                        {
                            apps.RemoveAt(n1);
                            apps.Add(TranslateModel(e));
                        }
                    }
                });
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
                    if (app.SessionState == SessionState.Disconnected && app.ID.Equals(e.Session.ID, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var device = SessionHandler.DeviceEnumerator.GetDeviceById(Identifier.GetDeviceId(e.Session.ID));
                        if (apps.ElementAt(n) == null)
                            continue;
                        apps.RemoveAt(n);
                        var process = Process.GetProcessById((int) e.SessionControl.GetProcessID);
                        apps.Add(new SessionModel
                        {
                            ID = e.Session.ID,
                            Image = process.GetMainWindowIcon().ToImageSource(),
                            SessionState = SessionState.Active,
                            Name = $"{process.GetPreciseName()} - {device.FriendlyName}",
                            SessionMode = SessionMode.SESSION
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
                if(Sliders[e.Index].Applications.Any(x => x.ID == e.Session.ID))
                    return;
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
                SessionState = e.SessionState,
                SessionMode = session.SessionMode
            };

            if(e.SessionState == SessionState.Active)
            {
                if (session.SessionMode == SessionMode.DEVICE)
                {
                    var device = SessionHandler.DeviceEnumerator.GetDeviceById(session.ID);
                    model.Image = IconExtractor.ExtractFromIndex(device.IconPath).ToImageSource();
                    model.Name = device.FriendlyName;
                    device.Dispose();
                }
                else if (session.SessionMode == SessionMode.SESSION)
                {
                    var deviceID = Identifier.GetDeviceId(session.ID);
                    if (!SessionHandler.SessionEnumerators.ContainsKey(deviceID))
                    {
                        model.Image = ExtractedIcons.FailedIcon.ToImageSource();
                        model.Name = $"{session.Name}(Device not available)";
                    }
                    var audioSession = SessionHandler.SessionEnumerators[deviceID].GetById(session.ID);
                    var process = Process.GetProcessById((int)audioSession.GetProcessID);
                    var device = SessionHandler.DeviceEnumerator.GetDeviceById(deviceID);
                    model.Image = (process.GetMainWindowIcon() ?? ExtractedIcons.FailedIcon).ToImageSource();
                    model.Name = $"{process.GetPreciseName()} - {device.FriendlyName}";
                    audioSession.Dispose();
                    device.Dispose();
                }
                else if (session.SessionMode == SessionMode.DEFAULT_MULTIMEDIA)
                {
                    if (session.DataFlow == DataFlow.Capture)
                    {
                        model.Name = "Default Microphone";
                        model.Image = ExtractedIcons.MicIcon.ToImageSource();
                    }
                    else
                    {
                        model.Name = "Default Speaker";
                        model.Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                    }
                }
                else if (session.SessionMode == SessionMode.DEFAULT_COMMUNICATION)
                {
                    if (session.DataFlow == DataFlow.Capture)
                    {
                        model.Name = "Default Communication Microphone";
                        model.Image = ExtractedIcons.MicIcon.ToImageSource();
                    }
                    else
                    {
                        model.Name = "Default Communication Speaker";
                        model.Image = ExtractedIcons.SpeakerIcon.ToImageSource();
                    }
                }
            }
            else if(e.SessionState == SessionState.Disconnected)
            {
                try
                {
                    var device = SessionHandler.DeviceEnumerator.GetDeviceById(Identifier.GetDeviceId(session.ID));
                    model.Image = ExtractedIcons.FailedIcon.ToImageSource();
                    model.Name = $"{session.Name} - {device.FriendlyName}(Not Active)";
                }
                catch
                {
                    model.Image = ExtractedIcons.FailedIcon.ToImageSource();
                    model.Name = $"{session.Name} - (Unknown device)";
                }
            }            
            else if(e.SessionState == SessionState.DeviceNotDetected)
            {
                model.Image = ExtractedIcons.FailedIcon.ToImageSource();
                model.Name = $"{session.Name} - (Unknown device)";
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
            _windowManager.ShowDialogAsync(addViewModel);
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
            if(session.SessionMode == SessionMode.SESSION || session.SessionMode == SessionMode.DEVICE)
                apps.Remove(apps.First(x=> x.ID == session.ID));
            else if (session.SessionMode == SessionMode.DEFAULT_MULTIMEDIA && session.DataFlow == DataFlow.Render)
                apps.Remove(apps.First(x => x.SessionMode == SessionMode.DEFAULT_MULTIMEDIA && x.DataFlow == DataFlow.Render));
            else if (session.SessionMode == SessionMode.DEFAULT_MULTIMEDIA && session.DataFlow == DataFlow.Capture)
                apps.Remove(apps.First(x => x.SessionMode == SessionMode.DEFAULT_MULTIMEDIA && x.DataFlow == DataFlow.Capture));
            else if (session.SessionMode == SessionMode.DEFAULT_COMMUNICATION && session.DataFlow == DataFlow.Render)
                apps.Remove(apps.First(x => x.SessionMode == SessionMode.DEFAULT_COMMUNICATION && x.DataFlow == DataFlow.Render));
            else if (session.SessionMode == SessionMode.DEFAULT_COMMUNICATION && session.DataFlow == DataFlow.Capture)
                apps.Remove(apps.First(x => x.SessionMode == SessionMode.DEFAULT_COMMUNICATION && x.DataFlow == DataFlow.Capture));
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
            model.MuteOut = !model.MuteOut;
        }

        public void EditNameClicked(object sender)
        {
            var sliderModel = sender as SliderModel;
            sliderModel.IsEditing = !sliderModel.IsEditing;
        }
        
        public void ConfirmEdit(object sender)
        {
            if (!(sender is ButtonModel buttonModel))
                return;
            if(buttonModel.IsEditing)
                buttonModel.IsEditing = !buttonModel.IsEditing;
        }

        #endregion
    }
}