using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using NAudio.Wave;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Helpers.Overlay;
using SoundMixerSoftware.Models;
using SoundMixerSoftware.Win32.Interop.Constant;
using SoundMixerSoftware.Win32.Interop.Method;
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

        public static MainViewModel Instance => IoC.Get<MainViewModel>();

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

        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainViewModel()
        {
            if(ConfigHandler.ConfigStruct.Application.HideOnStartup)
                User32.ShowWindow(Bootstrapper.Instance.MainWindowHandle, SW.SW_HIDE);
            
            RuntimeHelpers.RunClassConstructor(typeof(ThemeManager).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(OverlayHandler).TypeHandle);
        }

        #endregion

        #region Overriden Methods

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Tabs.Add(IoC.Get<ManagerViewModel>());
            Tabs.Add(IoC.Get<SlidersViewModel>());
            Tabs.Add(IoC.Get<ButtonsViewModel>());
            Tabs.Add(IoC.Get<DevicesViewModel>());
            Tabs.Add(IoC.Get<PluginViewModel>());
            Tabs.Add(IoC.Get<SettingsViewModel>());
            
            var configTab = ConfigHandler.ConfigStruct.Application.SelectedTab;
            SelectedTab = Tabs.FirstOrDefault(x => x.Uuid == configTab) ?? Tabs[0];

            var settingsTab = SettingsViewModel.Instance;
            settingsTab.SelectedTab = settingsTab.Tabs.FirstOrDefault(x => x.Uuid == configTab) ?? settingsTab.Tabs[0];
            return base.OnInitializeAsync(cancellationToken);
        }

        #endregion
    }
}