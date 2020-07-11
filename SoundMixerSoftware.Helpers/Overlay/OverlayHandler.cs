using System;
using System.Collections.Generic;
using SoundMixerSoftware.Helpers.Config;
using SoundMixerSoftware.Overlay.OverlayWindow;

namespace SoundMixerSoftware.Helpers.Overlay
{
    public static class OverlayHandler
    {
        #region Static Properties

        private static List<AbstractOverlayWindow> _overlays = new List<AbstractOverlayWindow>();
        
        #endregion
        
        #region Static Constructor
        
        static OverlayHandler()
        {
            var fadeTime = ConfigHandler.ConfigStruct.Overlay.OverlayFadeTime;
            _overlays.Add(new MuteWindow(fadeTime));
            _overlays.Add(new VolumeOverlay(fadeTime));
            _overlays.Add(new CenterTextWindow(fadeTime));
        }
        
        #endregion

        #region Static Methods

        public static void ShowVolume(float volume)
        {
            HandleOverlay<VolumeOverlay>((window, n) => window.Volume = volume);
        }

        public static void ShowMute(bool mute)
        {
            HandleOverlay<MuteWindow>((window, n) => window.IsMuted = mute);
        }

        public static void ShowText(string text, int showTime = -1, float fontSize = -1.0F)
        {
            HandleOverlay<CenterTextWindow>((window, n) =>
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
            foreach (var overlay in _overlays)
                overlay.FadeTime = fadeTime;
        }

        private static void HandleOverlay<T>(Action<T, int> valueChange)
        {
            if (!ConfigHandler.ConfigStruct.Overlay.EnableOverlay)
                return;
            for (var n = 0;n < _overlays.Count; n++)
            {
                var overlay = _overlays[n];
                if (overlay is T genericOverlay)
                {
                    valueChange(genericOverlay, n);
                    overlay.ShowWindow();
                }
                else
                    overlay.HideWindow();
            }
        }
        
        #endregion
    }
}