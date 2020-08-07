using System.Windows.Media;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions;

namespace SoundMixerSoftware.Models
{
    public interface ISessionModel
    {
        string ID { get; set; }
        string Name { get; set; }
        ImageSource Image { get; set; }
    }
    
    public class DefaultDeviceModel : ISessionModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public DefaultDeviceMode Mode { get; set; }
        public DataFlow DataFlow { get; set; }
    }

    public class AudioDeviceModel : ISessionModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public DataFlow DataFlow { get; set; }
        public Role Role { get; set; }
    }
    
    public class AudioSessionModel : ISessionModel{
        public string ID { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
    }
    
}