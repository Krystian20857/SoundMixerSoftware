using System;
using MaterialDesignThemes.Wpf;

namespace SoundMixerSoftware.Models
{
    public class TabModel : ITabModel
    {
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }
        public Guid Uuid { get; set; }

        public static TabModel CreateModel(ITabModel tabModel) => new TabModel {Name = tabModel.Name, Icon = tabModel.Icon, Uuid = tabModel.Uuid};
    }
}