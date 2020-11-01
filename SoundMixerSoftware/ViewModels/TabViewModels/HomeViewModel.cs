using System;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class HomeViewModel : ITabModel
    {
        
        #region Private Fields

        private ITabModel _selectedView;
        
        #endregion
        
        #region Implemented Properties

        public string Name { get; set; } = "Home";
        public PackIconKind Icon { get; set; } = PackIconKind.Home;
        public Guid Uuid { get; set; } = new Guid("640E7BCD-282A-4679-9026-56EA45DF41BE");
        
        #endregion
        
        #region Public Properties

        public static HomeViewModel Instance => IoC.Get<HomeViewModel>();
        public BindableCollection<ITabModel> Tabs { get; set; } = new BindableCollection<ITabModel>();

        public ITabModel SelectedView
        {
            get => _selectedView;
            set
            {
                _selectedView = value;
                MainViewModel.Instance.Content = value;
            }
        } 

        #endregion
        
        #region Constructor

        public HomeViewModel()
        {
            Tabs.Add(IoC.Get<ManagerViewModel>());
            Tabs.Add(IoC.Get<SlidersViewModel>());
            Tabs.Add(IoC.Get<ButtonsViewModel>());
            Tabs.Add(IoC.Get<DevicesViewModel>());
            Tabs.Add(IoC.Get<PluginViewModel>());
            Tabs.Add(IoC.Get<SettingsViewModel>());
        }
        
        #endregion
        
        #region Events
        
        
        
        #endregion
    }
}