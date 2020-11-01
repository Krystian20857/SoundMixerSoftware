using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MaterialDesignExtensions.Model;
using SoundMixerSoftware.Common.Utils.Application;
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

        private ITabModel content;

        #endregion
        
        #region Public Properties

        public static MainViewModel Instance { get; private set; }

        public ITabModel Content
        {
            get => content;
            set
            {
                content = value;
                NotifyOfPropertyChange(nameof(Content));
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
            
            RuntimeHelpers.RunClassConstructor(typeof(ThemeManager).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(OverlayHandler).TypeHandle);
        }

        #endregion

        #region Overriden Methods

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Content = HomeViewModel.Instance;
            Initialized?.Invoke(this, EventArgs.Empty);
            return base.OnInitializeAsync(cancellationToken);
        }

        #endregion
        
        #region Events

        public void BackClicked()
        {
            var homePage = HomeViewModel.Instance;
            if(Content?.Uuid != homePage.Uuid)
                Content = homePage;
        }
        
        public void GithubClicked()
        {
            Process.Start("https://github.com/Krystian20857/SoundMixerSoftware/");
        }
        
        #endregion
    }
}