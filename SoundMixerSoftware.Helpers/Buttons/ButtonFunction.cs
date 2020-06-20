using System;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Common.Utils.Enum;

namespace SoundMixerSoftware.Helpers.Buttons
{
    public enum ButtonFunction
    {
        [ValueName("No Function")] NoFunction,
        [ValueName("Mute Output Device")] MuteDefaultOutput,
        [ValueName("Mute Input Device")] MuteDefaultInput,
        [ValueName("Go To Previous Track")] PrevTrack,
        [ValueName("Go To Next Track")] NextTrack,
        [ValueName("Play/Pause")] PausePlay,
        [ValueName("Stop")] Stop
    }
}