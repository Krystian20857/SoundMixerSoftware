using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Overlay.OverlayWindow;

namespace SoundMixerSoftware.Helpers.Overlay
{
    public static class OverlayHandler
    {
        #region Static Properties

        public static OverlaySwitcher Switcher { get; set; } = new OverlaySwitcher(() => ConfigHandler.ConfigStruct.Overlay.EnableOverlay);
        public static int FadeTime => ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime;

        #endregion
        
        #region Static Constructor
        
        static OverlayHandler()
        {
            Switcher.SetValue("mute-overlay", new MuteWindow(FadeTime));
            Switcher.SetValue("volume-overlay", new VolumeOverlay(FadeTime));
            Switcher.SetValue("center-text-overlay", new CenterTextWindow(FadeTime));
        }
        
        #endregion

        #region Static Methods

        public static void ShowVolume(float volume)
        {
            Switcher.HandleOverlay<VolumeOverlay>((key, window) => window.Volume = volume);
        }

        public static void ShowMute(bool mute)
        {
            Switcher.HandleOverlay<MuteWindow>((key, window) => window.IsMuted = mute);
        }

        public static void ShowText(string text, int showTime = -1, float fontSize = -1.0F)
        {
            Switcher.HandleOverlay<CenterTextWindow>((key, window) =>
            {
                window.Text = text;
                if(showTime != -1)
                    window.TempFadeTime = showTime;
                if ((int)fontSize != -1)
                    window.FontSize = fontSize;
            });
        }

        public static void SetFadeTime(int fadeTime)
        {
            foreach (var overlay in Switcher.GetValues())
                overlay.FadeTime = fadeTime;
        }
        
        
        #endregion
    }
}