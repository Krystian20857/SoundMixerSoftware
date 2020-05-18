using Caliburn.Micro;

namespace SoundMixerSoftware.Models
{
    public class ButtonModel
    {
        public string Name { get; set; }
        public BindableCollection<string> Function { get; set; } = new BindableCollection<string>();
        public string SelectedItem { get; set; }
    }
}