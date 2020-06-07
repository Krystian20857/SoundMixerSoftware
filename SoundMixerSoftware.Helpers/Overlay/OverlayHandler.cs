using SoundMixerSoftware.Overlay.OverlayWindow;

namespace SoundMixerSoftware.Helpers.Overlay
{
    public static class OverlayHandler
    {
        public static MuteWindow MuteOverlay { get; } = new MuteWindow();
        public static VolumeOverlay VolumeOverlay { get; } = new VolumeOverlay();
    }
}