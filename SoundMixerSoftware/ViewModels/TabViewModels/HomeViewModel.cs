using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;

namespace SoundMixerSoftware.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        
        #region Private Fields

        private object _selectedView;
        
        #endregion
        
        #region Implemented Properties
        
        
        #endregion
        
        #region Public Properties

        public static HomeViewModel Instance => IoC.Get<HomeViewModel>();
        public BindableCollection<object> Tabs { get; set; } = new BindableCollection<object>();

        public object SelectedView
        {
            get => _selectedView;
            set
            {
                _selectedView = value;
                MainViewModel.Instance.Content = value;
                _selectedView = null;
                OnPropertyChanged(nameof(SelectedView));
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
        
        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}