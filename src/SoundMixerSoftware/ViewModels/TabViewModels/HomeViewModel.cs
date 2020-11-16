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
                if(value != null)
                    MainViewModel.Instance.Content = value;
                OnPropertyChanged(nameof(SelectedView));
            }
        } 

        #endregion
        
        #region Constructor

        public HomeViewModel()
        {
            Tabs.Add(ManagerViewModel.Instance);
            Tabs.Add(SlidersViewModel.Instance);
            Tabs.Add(ButtonsViewModel.Instance);
            Tabs.Add(DevicesViewModel.Instance);
            Tabs.Add(PluginViewModel.Instance);
            Tabs.Add(UpdateViewModel.Instance);
            Tabs.Add(SettingsViewModel.Instance);
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