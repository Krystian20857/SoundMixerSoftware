using Caliburn.Micro;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Class contains properties used in button view creating.
    /// </summary>
    public class ButtonModel
    {
        /// <summary>
        /// Button name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Buttons base functions.
        /// </summary>
        public BindableCollection<string> Function { get; set; } = new BindableCollection<string>();
        /// <summary>
        /// Button Function.
        /// </summary>
        public string SelectedItem { get; set; }
    }
}