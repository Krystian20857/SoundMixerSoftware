
using System;
using System.Collections.Generic;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using NLog;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Profile;
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
        public Guid Uuid { get; set; } = new Guid("5E23DDD2-71B1-44C3-BC81-FF6BD7901674");

        #endregion
        
        #region Public Properties
        
        public static ManagerViewModel Instance => IoC.Get<ManagerViewModel>();

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
            var profileOrder = ConfigHandler.ConfigStruct.Application.ProfilesOrder;
            bool saveConfig = false;
            
            foreach (var profile in ProfileHandler.ProfileManager.Profiles)
            {
                var uuid = profile.Key;
                if (profileOrder.Contains(uuid))
                    continue;
                profileOrder.Add(uuid);
                saveConfig = true;
            }
            
            foreach (var uuid in profileOrder)
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
                if (model.Guid == ConfigHandler.ConfigStruct.Application.SelectedProfile)
                {
                    model.Selected = true;
                    ProfileHandler.OnProfileChanged(model.Guid);
                }
                Profiles.Add(model);
            }

            foreach (var profile in profilesToRemove)
                profileOrder.Remove(profile);
            if(profilesToRemove.Count > 0 || saveConfig)
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
            //foreach (var profile in Profiles)
                //profile.Selected = profile.Guid == e.Uuid;
        }
        
        /// <summary>
        /// Occurs when Copy Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void SelectClick(object sender)
        {
            var model = sender as ProfileModel;
            if (!model.Selected)
                model.Selected = true;
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
            _windowManager.ShowDialogAsync(addWindow);
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
            ProfileHandler.ProfileManager.Remove(model.Guid);
            ProfileHandler.OnProfileRemoved(model.Guid);
            ConfigHandler.ConfigStruct.Application.ProfilesOrder.Remove(model.Guid);
            ConfigHandler.SaveConfig();
        }
        
        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void AddClick(object sender)
        {
            var addWindow = new ProfileAddViewModel();
            _windowManager.ShowDialogAsync(addWindow);
            var model = addWindow.CreatedProfile;
            if (model.IsInitialized())
            {
                Profiles.Add(model);
                ProfileHandler.OnProfileCreated(model.Guid);
                ConfigHandler.ConfigStruct.Application.ProfilesOrder.Add(model.Guid);
                ConfigHandler.SaveConfig();
            }
        }
        
        #endregion
    }
}