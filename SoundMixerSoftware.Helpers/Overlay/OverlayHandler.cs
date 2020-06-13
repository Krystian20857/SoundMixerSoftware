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
            _overlays.Add(new MuteWindow(ConfigHandler.ConfigStruct.OverlayFadeTime));
            _overlays.Add(new VolumeOverlay(ConfigHandler.ConfigStruct.OverlayFadeTime));
        }
        
        #endregion

        #region Static Methods

        public static void ShowVolume(float volume)
        {
            HandleOverlay<VolumeOverlay>(window => window.Volume = volume);
        }

        public static void ShowMute(bool mute)
        {
            HandleOverlay<MuteWindow>(window => window.IsMuted = mute);
        }

        public static void SetFadeTime(int fadeTime)
        {
            foreach (var overlay in _overlays)
                overlay.FadeTime = fadeTime;
        }

        private static void HandleOverlay<T>(Action<T> valueChange)
        {
            if (!ConfigHandler.ConfigStruct.EnableOverlay)
                return;
            foreach (var overlay in _overlays)
            {
                if (overlay is T genericOverlay)
                {
                    valueChange(genericOverlay);
                    overlay.ShowWindow();
                }
                else
                    overlay.HideWindow();
            }
        }
        
        #endregion
    }
}