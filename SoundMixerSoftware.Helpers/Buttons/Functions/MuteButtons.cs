using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Overlay;

namespace SoundMixerSoftware.Helpers.Buttons.Functions
{
    public class DefaultSpeakerMuteButton : IButton
    {
        public ButtonFunction Key { get; } = ButtonFunction.MuteDefaultOutput;
        public string Name { get; } = "Mute Default Speaker";
        
        public bool Clicked(int index)
        {
            var defaultOutput = SessionHandler.DeviceEnumerator.DefaultOutput;
            var audioEndpoint = defaultOutput.AudioEndpointVolume;
            audioEndpoint.Mute = !audioEndpoint.Mute;
            OverlayHandler.ShowMute(audioEndpoint.Mute);
            return true;
        }
    }
    
    public class DefaultMicMuteButton : IButton
    {
        public ButtonFunction Key { get; } = ButtonFunction.MuteDefaultInput;
        public string Name { get; } = "Mute Default Microphone";
        
        public bool Clicked(int index)
        {
            var defaultInput = SessionHandler.DeviceEnumerator.DefaultInput;
            var audioEndpoint = defaultInput.AudioEndpointVolume;
            audioEndpoint.Mute = !audioEndpoint.Mute;
            OverlayHandler.ShowMute(audioEndpoint.Mute);
            return true;
        }
    }
}