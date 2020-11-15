using System;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Overlay.OverlayWindow;

namespace SoundMixerSoftware.Framework.Overlay
{
    public static class OverlayHandler
    {
        #region Static Properties

        public static OverlaySwitcher Switcher { get; } = new OverlaySwitcher(() => ConfigHandler.ConfigStruct.Overlay.EnableOverlay);
        public static int FadeTime => ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime.Milliseconds;

        #endregion
        
        #region Static Constructor
        
        static OverlayHandler()
        {
            Switcher.SetValue("mute-overlay", new MuteOverlay(FadeTime));
            Switcher.SetValue("volume-overlay", new VolumeOverlay(FadeTime));
            Switcher.SetValue("center-text-overlay", new CenterTextOverlay(FadeTime));
            Switcher.SetValue("profile-overlay", new ProfileOverlay(FadeTime));
        }
        
        #endregion

        #region Static Methods

        public static void ShowVolume(float volume)
        {
            Switcher.HandleOverlay<VolumeOverlay>((key, window) => window.Volume = volume);
        }

        public static void ShowMute(bool mute)
        {
            Switcher.HandleOverlay<MuteOverlay>((key, window) => window.IsMuted = mute);
        }

        public static void ShowText(string text, int showTime = -1, float fontSize = -1.0F)
        {
            Switcher.HandleOverlay<CenterTextOverlay>((key, window) =>
            {
                window.Text = text;
                if(showTime != -1)
                    window.TempFadeTime = showTime;
                if ((int)fontSize != -1)
                    window.FontSize = fontSize;
            });
        }
        
        public static void ShowProfile(Guid profileUUID){
            Switcher.HandleOverlay<ProfileOverlay>((key, window) =>
            {
                if(ProfileHandler.ProfileManager.Profiles.TryGetValue(profileUUID, out var profile))
                    window.DisplayName = profile.Name;
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