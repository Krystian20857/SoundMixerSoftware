using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace SoundMixerSoftware.Models
{
    public interface ITabModel
    {
        string Name { get; set; }
        PackIconKind Icon { get; set; }
    }
}