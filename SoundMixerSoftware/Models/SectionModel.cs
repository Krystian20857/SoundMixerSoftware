using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;

namespace SoundMixerSoftware.Models
{
    public class SectionModel : INotifyPropertyChanged
    {
        #region Private Fields

        private string _name;
        private ISessionModel _selectedSession;
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Display name of section.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        /// <summary>
        /// Identifier of section.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Sessions.
        /// </summary>
        public BindableCollection<ISessionModel> Sessions { get; set; } = new BindableCollection<ISessionModel>();

        /// <summary>
        /// Selected session.
        /// </summary>
        public ISessionModel SelectedSession
        {
            get => _selectedSession;
            set
            {
                _selectedSession = value;
                OnPropertyChanged(nameof(SelectedSession));
            }
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