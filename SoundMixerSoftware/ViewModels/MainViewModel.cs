using Caliburn.Micro;
using SoundMixerSoftware.Models;
using Screen = Caliburn.Micro.Screen;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of main window
    /// </summary>
    public class MainViewModel : Screen
    {
        #region Private Fields
        
        private BindableCollection<ITabModel> _tabs = new BindableCollection<ITabModel>();
        private ITabModel _selectedTab;
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Collection of tabs.
        /// </summary>
        public BindableCollection<ITabModel> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = Tabs;
                NotifyOfPropertyChange(() => Tabs);
            }
        }

        /// <summary>
        /// Currently selected tab.
        /// </summary>
        public ITabModel SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                NotifyOfPropertyChange(() => _selectedTab);
            }
        }

        #endregion
        
        #region Constructor
        
        public MainViewModel()
        {
            Tabs.Add(new ManagerViewModel());
            Tabs.Add(new SlidersViewModel());
            Tabs.Add(new ButtonsViewModel());
            Tabs.Add(new DevicesViewModel());
            Tabs.Add(new SettingsViewModel());

            SelectedTab = Tabs[0];
        }
        
        #endregion
    }
}