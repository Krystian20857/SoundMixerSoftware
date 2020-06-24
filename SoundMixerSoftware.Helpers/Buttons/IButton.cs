namespace SoundMixerSoftware.Helpers.Buttons
{
    public interface IButton
    {
        ButtonFunction Key { get; }
        string Name { get; }
        bool Clicked(int index);
    }
}