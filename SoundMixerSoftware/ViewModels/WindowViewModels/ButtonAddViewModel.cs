using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ButtonAddViewModel : Screen
    {

        #region Private Fields

        private object _content;
        
        #endregion
        
        #region Public Properties

        public static ButtonAddViewModel Instance => IoC.Get<ButtonAddViewModel>();

        public int Index { get; set; }

        public object Content
        {
            get => _content;
            set
            {
                _content = value;
                AddButtonVisible = !(value is HomeButtonViewModel);
                NotifyOfPropertyChange(nameof(Content));
                NotifyOfPropertyChange(nameof(AddButtonVisible));
            }
        }

        public bool AddButtonVisible { get; set; }

        #endregion
        
        #region Constructor

        public ButtonAddViewModel()
        {
            
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Content = HomeButtonViewModel.Instance;
            return base.OnInitializeAsync(cancellationToken);
        }

        #endregion
        
        #region Events

        public void BackClicked()
        {
            if(Content is HomeButtonViewModel)
                return;
            Content = HomeButtonViewModel.Instance;
        }

        /// <summary>
        /// Occurs when global add button has clicked.
        /// </summary>
        public void AddClicked()
        {
            var selectedModel = HomeButtonViewModel.Instance.SelectedView;
            if(!(selectedModel is IButtonAddModel model))
                return;
            if(model.AddClicked(Index))
                TryCloseAsync();
        }
        
        #endregion
    }
}