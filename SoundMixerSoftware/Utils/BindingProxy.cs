using System.Windows;

namespace SoundMixerSoftware.Utils
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty Dataproperty = DependencyProperty.Register(nameof(Data),
            typeof(object),
            typeof(BindingProxy),
            new PropertyMetadata(null));
        
        public object Data
        {
            get => GetValue(Dataproperty);
            set => SetValue(Dataproperty, value);
        }
        
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
        
    }
}