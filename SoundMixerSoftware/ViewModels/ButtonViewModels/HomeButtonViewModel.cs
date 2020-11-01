using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class HomeButtonViewModel : INotifyPropertyChanged
    {
         #region Provate Fields

         private object _selectedView;

         #endregion
         
         #region Implemented Properties

         public static HomeButtonViewModel Instance => IoC.Get<HomeButtonViewModel>();
         
         #endregion

         #region Public Properties

         public object SelectedView
         {
             get => _selectedView;
             set
             {
                 _selectedView = value;
                 if(value != null)
                    ButtonAddViewModel.Instance.Content = value;
                 OnPropertyChanged(nameof(SelectedView));
             }
         }

         public BindableCollection<IButtonAddModel> Tabs { get; } = new BindableCollection<IButtonAddModel>();

         #endregion
         
         #region Constrcutor

         public HomeButtonViewModel()
         {
             Tabs.Add(new MediaButtonViewModel());
             Tabs.Add(new MuteButtonViewModel());
             Tabs.Add(new VolumeButtonViewModel());
             Tabs.Add(new KeystrokeFunctionViewModel());
             Tabs.Add(new ProfileButtonViewModel());
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