namespace SoundMixerAppv2.Common.Config
{
    public interface IConfigStruct<out T>
    {
        T GetSampleConfig();
        T Copy();
    }
}