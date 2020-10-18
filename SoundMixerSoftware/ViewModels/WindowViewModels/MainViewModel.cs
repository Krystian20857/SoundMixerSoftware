using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Overlay;
using SoundMixerSoftware.Models;

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

        public static MainViewModel Instance { get; private set; }

        /// <summary>
        /// Collection of tabs.
        /// </summary>
        public BindableCollection<ITabModel> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;
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
        
        #region Events

        public event EventHandler Initialized;

        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainViewModel()
        {
            Instance = this;
            
            Tabs.Add(IoC.Get<ManagerViewModel>());
            Tabs.Add(IoC.Get<SlidersViewModel>());
            Tabs.Add(IoC.Get<ButtonsViewModel>());
            Tabs.Add(IoC.Get<DevicesViewModel>());
            Tabs.Add(IoC.Get<PluginViewModel>());
            Tabs.Add(IoC.Get<SettingsViewModel>());
            
            RuntimeHelpers.RunClassConstructor(typeof(ThemeManager).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(OverlayHandler).TypeHandle);
        }

        #endregion

        #region Overriden Methods

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            var configTab = ConfigHandler.ConfigStruct.Application.SelectedTab;
            SelectedTab = Tabs.FirstOrDefault(x => x.Uuid == configTab) ?? Tabs[0];

            var settingsTab = SettingsViewModel.Instance;
            settingsTab.SelectedTab = settingsTab.Tabs.FirstOrDefault(x => x.Uuid == configTab) ?? settingsTab.Tabs[0];

            Initialized?.Invoke(this, EventArgs.Empty);
            return base.OnInitializeAsync(cancellationToken);
        }

        #endregion
    }
}