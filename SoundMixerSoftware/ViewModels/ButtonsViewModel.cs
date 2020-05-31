using System.Threading.Tasks;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Helpers.Buttons;
using SoundMixerSoftware.Helpers.Device;
using SoundMixerSoftware.Helpers.Profile;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    /// <summary>
    /// View model of buttons item in main window.
    /// </summary>
    public class ButtonsViewModel : ITabModel
    {
        #region Private Fields
        
        private BindableCollection<ButtonModel> _buttons = new BindableCollection<ButtonModel>();
        
        #endregion
        
        #region Public Propeties

        /// <summary>
        /// Collection of ListView models.
        /// </summary>
        public BindableCollection<ButtonModel> Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
            }
        }

        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion
        
        #region Constructor

        public ButtonsViewModel()
        {
            Name = "Buttons";
            Icon = PackIconKind.GestureTapButton;
            
            ProfileHandler.ProfileChanged += ProfileHandlerOnProfileChanged;
            
            ButtonHandler.Initialize();
            CreateButtons();
        }

        #endregion
        
        #region Private Events
        
        /// <summary>
        /// When profile changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileHandlerOnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            CreateButtons();
        }

        /// <summary>
        /// Occurs when function of button has changed.
        /// </summary>
        /// <param name="sender"></param>
        public void FunctionChanged(object sender)
        {
            if (!(sender is ButtonModel model)) return;
            var function = EnumNameConverter.GetValue<ButtonFunction>(model.SelectedItem);
            ProfileHandler.SelectedProfile.Buttons[model.Index].Function = function;
            ProfileHandler.ProfileManager.Save(ProfileHandler.SelectedGuid);
        }

        /// <summary>
        /// Occurs when check button has clicked.
        /// </summary>
        /// <param name="sender"></param>
        public void ButtonCheck(object sender)
        {
            if (!(sender is ButtonModel model)) return;
            var index = model.Index;
            foreach (var device in DeviceHandlerGlobal.ConnectedDevice)
            {
                var blinkTask = Task.Run(async () =>
                {
                    DeviceNotifier.LightButton(device.Key, (byte) index, true);
                    await Task.Delay(500);
                    DeviceNotifier.LightButton(device.Key, (byte) index, false);
                    await Task.Delay(500);
                });
            }
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Creates buttons.
        /// </summary>
        private void CreateButtons()
        {
            var enumValueNames = EnumNameConverter.GetNames(typeof(ButtonFunction));
            var buttons = ProfileHandler.SelectedProfile.Buttons;
            
            Buttons.Clear();
            for (var n = 0; n < ProfileHandler.SelectedProfile.ButtonCount; n++)
            {
                var buttonModel = new ButtonModel
                {
                    Name = $"Button {n + 1}",
                    Function = new BindableCollection<string>(enumValueNames),
                    Index = n
                };
                if (buttons.Count <= n)
                {
                    buttons.Add(new ButtonStruct
                    {
                        Index = n,
                        Function = ButtonFunction.NoFunction
                    });
                    ProfileHandler.ProfileManager.Save(ProfileHandler.SelectedGuid);
                }

                var function = buttons[n].Function;
                buttonModel.SelectedItem = buttonModel.Function[(int) function];
                Buttons.Add(buttonModel);
            }
        }
        
        #endregion
    }
}