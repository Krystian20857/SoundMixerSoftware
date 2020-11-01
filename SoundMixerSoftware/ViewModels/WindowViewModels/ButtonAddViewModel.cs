using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ButtonAddViewModel : Screen
    {

        #region Public Properties

        public static ButtonAddViewModel Instance => IoC.Get<ButtonAddViewModel>();

        public int Index { get; set; }
        public IButtonAddModel SelectedTab { get; set; }
        public BindableCollection<IButtonAddModel> Tabs { get; set; } = new BindableCollection<IButtonAddModel>();

        #endregion
        
        #region Constructor

        public ButtonAddViewModel() { }
        
        #endregion
        
        #region Public Methods

        public void AddView(IButtonAddModel viewModel)
        {
            Tabs.Add(viewModel);
        }

        #endregion
        
        #region Private Events

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            AddView(new MediaButtonViewModel());
            AddView(new MuteButtonViewModel());
            AddView(new VolumeButtonViewModel());
            AddView(new KeystrokeFunctionViewModel());
            AddView(new ProfileButtonViewModel());
            
            if (Tabs.Count > 0)
                SelectedTab = Tabs[0];
            
            return base.OnInitializeAsync(cancellationToken);
        }

        /// <summary>
        /// Occurs when global add button has clicked.
        /// </summary>
        public void AddClicked()
        {
            if(SelectedTab.AddClicked(Index))
                TryCloseAsync();
        }
        
        #endregion
    }
}