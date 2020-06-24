using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Helpers.Buttons.Functions
{
    public class PlayPauseButton : IButton
    {
        public ButtonFunction Key { get; } = ButtonFunction.PausePlay;
        public string Name { get; } = "Media Play/Pause";
        
        public bool Clicked(int index)
        {
            MediaControl.PauseResume();
            return true;
        }
        
    }
    
    public class StopMediaButton : IButton
    {
        public ButtonFunction Key { get; } = ButtonFunction.Stop;
        public string Name { get; } = "Media Stop";
        
        public bool Clicked(int index)
        {
            MediaControl.Stop();
            return true;
        }
        
    }
    
    public class PrevTrackButton : IButton
    {
        public ButtonFunction Key { get; } = ButtonFunction.PrevTrack;
        public string Name { get; } = "Media Previous Track";
        
        public bool Clicked(int index)
        {
            MediaControl.PrevTrack();
            return true;
        }
        
    }
    
    public class NextTrackButton : IButton
    {
        public ButtonFunction Key { get; } = ButtonFunction.NextTrack;
        public string Name { get; } = "Media Next Track";
        
        public bool Clicked(int index)
        {
            MediaControl.NextTrack();
            return true;
        }
        
    }
}