using System;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Framework.Audio;
using SoundMixerSoftware.Framework.Audio.VirtualSessions;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.SliderConverter;
using SoundMixerSoftware.Framework.SliderConverter.Converters;
using SoundMixerSoftware.Models;
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
            
            SessionHandler.RegisterCreator(VirtualSession.KEY, new VirtualSessionCreator());
            SessionHandler.RegisterCreator(DeviceSession.KEY, new DeviceSessionCreator());
            SessionHandler.RegisterCreator(DefaultDeviceSession.KEY, new DefaultDeviceSessionCreator());
            SessionHandler.RegisterCreator(ForegroundSession.KEY, new ForegroundSessionCreator());
            SessionHandler.RegisterCreator(ProcessSession.KEY, new ProcessSessionCreator());
            
            ConverterHandler.RegisterCreator("log_converter", new LogConverterCreator());

            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;

            if(ProfileHandler.SelectedProfile != null)
                UpdateProfile();
        }

        #endregion

        #region Private Events

        private void UpdateProfile()
        {
            SessionHandler.VirtualSessionCreated -= SessionHandlerOnSessionCreated;
            SessionHandler.VirtualSessionRemoved -= SessionHandlerOnSessionRemoved;
            SessionHandler.ReloadAll();

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
            
            SessionHandler.VirtualSessionCreated += SessionHandlerOnSessionCreated;
            SessionHandler.VirtualSessionRemoved += SessionHandlerOnSessionRemoved;
            SessionHandler.CreateSessions();
        }

        private void SessionHandlerOnSessionRemoved(object sender, SessionArgs e)
        {
            var index = e.Index;
            if (index >= Sliders.Count)
                return;
            var slider = Sliders[index];
            slider.Applications.Remove(e.Session);
        }

        private void SessionHandlerOnSessionCreated(object sender, SessionArgs e)
        {
            var index = e.Index;
            if (index >= Sliders.Count)
                return;
            var slider = Sliders[index];
            slider.Applications.Add(e.Session);
        }

        private void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            UpdateProfile();
        }
        
        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void AddClick(object sender)
        {
            var model = sender as SliderModel;
            var addViewModel = SessionAddViewModel.Instance;
            addViewModel.SliderIndex = model.Index;
            _windowManager.ShowDialogAsync(addViewModel);
        }

        public void AddExtensionClick(object sender)
        {
            var model = sender as SliderModel;
            var extensionViewModel = ExtensionAddViewModel.Instance;
            extensionViewModel.SliderIndex = model.Index;
            _windowManager.ShowDialogAsync(extensionViewModel);
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
            var sliders = ProfileHandler.SelectedProfile.Sliders[session.Index].Sessions;
            sliders.RemoveAll(x => x.UUID == session.UUID);
            ProfileHandler.SaveSelectedProfile();
            SessionHandler.RemoveSession(session.Index, session);
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