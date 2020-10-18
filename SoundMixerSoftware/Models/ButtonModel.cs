using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Profile;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Class contains properties used in button view creating.
    /// </summary>
    public class ButtonModel : INotifyPropertyChanged
    {
        #region Private Fields

        private bool _isEditing;
        private string _name;
        
        #endregion
        
        #region Priperties
        
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            } 
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                if (!value)
                {
                    ProfileHandler.SelectedProfile.Buttons[Index].Name = _name;
                    ProfileHandler.SaveSelectedProfile();
                }
                OnPropertyChanged(nameof(IsEditing));
            }
        }
        /// <summary>
        /// Buttons base functions.
        /// </summary>
        public BindableCollection<IButton> Functions { get; set; } = new BindableCollection<IButton>();
        /// <summary>
        /// Button Function.
        /// </summary>
        public IButton SelectedFunction { get; set; }
        /// <summary>
        /// Index of current button.
        /// </summary>
        public int Index { get; set; }

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