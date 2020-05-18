using System.Windows.Media;
using Caliburn.Micro;

namespace SoundMixerSoftware.Models
{
    public class SliderModel
    {
        public int Volume { get; set; }
        public BindableCollection<AppModel> Applications { get; set; } = new BindableCollection<AppModel>();
    }

    public class AppModel
    {
        public string App { get; set; }
        public ImageSource Image { get; set; }
    }
}