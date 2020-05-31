
using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using NLog;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;
using LogManager = NLog.LogManager;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View Model of Profiles tab in main window.
    /// </summary>
    public class ManagerViewModel : ITabModel
    {
        #region Current Class Logger

        /// <summary>
        /// Current Class Logger
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields
        
        private IWindowManager _windowManager = new WindowManager();
        private BindableCollection<ProfileModel> _profiles = new BindableCollection<ProfileModel>();
        
        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Collection of profiles.
        /// </summary>
        public BindableCollection<ProfileModel> Profiles
        {
            get => _profiles;
            set => _profiles = value;
        }

        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Create instance of profile manager.
        /// </summary>
        public ManagerViewModel()
        {
            Name = "Profiles";
            Icon = PackIconKind.AccountBoxMultipleOutline;
            var profilesToRemove = new List<Guid>();
            foreach (var uuid in ConfigHandler.ConfigStruct.ProfilesOrder)
            {
                if (!ProfileHandler.ProfileManager.Profiles.ContainsKey(uuid))
                {
                    profilesToRemove.Add(uuid);
                    Logger.Warn($"Profile with ID: {uuid} has not been found.");
                    continue;
                }
                var profile = ProfileHandler.ProfileManager.Profiles[uuid];
                var model = ProfileModel.CreateModel(profile);
                model.Guid = uuid;
                if (model.Guid == ConfigHandler.ConfigStruct.SelectedProfile)
                {
                    model.Selected = true;
                    ProfileHandler.OnProfileChanged(model.Guid);
                }
                Profiles.Add(model);
            }

            foreach (var profile in profilesToRemove)
                ConfigHandler.ConfigStruct.ProfilesOrder.Remove(profile);
            ConfigHandler.SaveConfig();

            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;
        }

        #endregion
        
        #region Private Events

        /// <summary>
        /// Occurs when profile has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            foreach (var profile in Profiles)
                profile.Selected = profile.Guid == e.Uuid;
        }
        
        /// <summary>
        /// Occurs when Copy Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void SelectClick(object sender)
        {
            var model = sender as ProfileModel;
            ProfileHandler.OnProfileChanged(model.Guid);
        }
        
        /// <summary>
        /// Occurs when Edit Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void EditClick(object sender)
        {
            var model = sender as ProfileModel;
            var index = Profiles.IndexOf(model);
            var addWindow = new ProfileAddViewModel(model);
            _windowManager.ShowDialog(addWindow);
            Profiles[index] = addWindow.CreatedProfile;
        }
        
        /// <summary>
        /// Occurs when Remove Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void RemoveClick(object sender)
        {
            var model = sender as ProfileModel;
            Profiles.Remove(model);
            var profiles = ProfileHandler.ProfileManager.Profiles;
            ProfileHandler.ProfileManager.Remove(model.Guid);
            ConfigHandler.ConfigStruct.ProfilesOrder.Remove(model.Guid);
            ConfigHandler.SaveConfig();
        }
        
        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void AddClick(object sender)
        {
            var addWindow = new ProfileAddViewModel();
            _windowManager.ShowDialog(addWindow);
            var model = addWindow.CreatedProfile;
            if (model.IsInitialized())
            {
                Profiles.Add(model);
                ConfigHandler.ConfigStruct.ProfilesOrder.Add(model.Guid);
                ConfigHandler.SaveConfig();
            }
        }
        
        #endregion
    }
}