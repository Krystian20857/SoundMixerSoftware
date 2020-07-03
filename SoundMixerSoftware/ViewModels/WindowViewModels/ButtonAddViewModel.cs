using System;
using Caliburn.Micro;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ButtonAddViewModel : Screen
    {

        #region Public Properties

        public int Index { get; set; }
        public IButtonAddModel SelectedTab { get; set; }
        public BindableCollection<IButtonAddModel> Tabs { get; set; } = new BindableCollection<IButtonAddModel>();

        #endregion
        
        #region Constructor

        public ButtonAddViewModel()
        {
            AddView(new MediaButtonViewModel());
            AddView(new MuteButtonViewModel());

            if (Tabs.Count > 0)
                SelectedTab = Tabs[0];
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Add button add view and viewModel using IoC container.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <exception cref="StackOverflowException">Using in ButtonAddViewModel constructor can cause this exception</exception>
        public static void AddViewStatic(IButtonAddModel viewModel)
        {
            var addViewModel = IoC.Get<ButtonAddViewModel>();
            addViewModel.Tabs.Add(viewModel);
        }

        public void AddView(IButtonAddModel viewModel)
        {
            Tabs.Add(viewModel);
        }

        #endregion
        
        #region Private Events

        /// <summary>
        /// Occurs when global add button has clicked.
        /// </summary>
        public void AddClicked()
        {
            SelectedTab.AddClicked(Index);
            TryCloseAsync();
        }
        
        #endregion
    }
}