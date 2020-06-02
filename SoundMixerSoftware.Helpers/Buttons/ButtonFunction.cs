using System;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Common.Utils.Enum;

namespace SoundMixerSoftware.Helpers.Buttons
{
    public enum ButtonFunction
    {
        [ValueName("No Function")] NoFunction,
        [ValueName("Mute Output Device")] MuteOutput,
        [ValueName("Mute Input Device")]MuteInput,
        [ValueName("Go To Previous Track")]PrevTrack,
        [ValueName("Go To Next Track")]NextTrack,
        [ValueName("Play/Pause")]PausePlay
    }
}