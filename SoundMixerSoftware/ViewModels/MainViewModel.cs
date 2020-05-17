using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Views;
using Screen = Caliburn.Micro.Screen;

namespace SoundMixerSoftware.ViewModels
{
    public class MainViewModel : Screen
    {
        #region Private Fields
        
        private BindableCollection<ITabModel> _tabs = new BindableCollection<ITabModel>();
        private ITabModel _selectedTab;
        
        #endregion
        
        #region Public Properties

        public BindableCollection<ITabModel> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = Tabs;
                NotifyOfPropertyChange(() => Tabs);
            }
        }

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
        }
        
        #endregion
    }
}