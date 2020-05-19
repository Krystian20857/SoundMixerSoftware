using System.Windows;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View Model of Profiles tab in main window.
    /// </summary>
    public class ManagerViewModel : ITabModel
    {
        #region Private Fields
        
        private BindableCollection<ProfileModel> _profiles = new BindableCollection<ProfileModel>();
        
        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Collection of profiles.
        /// </summary>
        public BindableCollection<ProfileModel> Profiles
        {
            get => _profiles;
            set => _profiles = value;
        }

        #endregion
        
        #region Constructor
        
        public ManagerViewModel()
        {
            Name = "Profiles";
            Icon = PackIconKind.AccountBoxMultipleOutline;
        }
        
        #endregion
        
        #region Private Events

        /// <summary>
        /// Occurs when Copy Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void CopyClick(object sender)
        {
            MessageBox.Show("Copy");
        }
        
        /// <summary>
        /// Occurs when Edit Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void EditClick(object sender)
        {
            MessageBox.Show("Edit");
        }
        
        /// <summary>
        /// Occurs when Remove Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void RemoveClick(object sender)
        {
            MessageBox.Show("Remove");
        }
        
        /// <summary>
        /// Occurs when Add Button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void AddClick(object sender)
        {
            MessageBox.Show("Add");
        }
        
        #endregion
    }
}