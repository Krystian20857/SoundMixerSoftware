namespace SoundMixerSoftware.Overlay.OverlayWindow
{
    public interface IOverlay
    {
        /// <summary>
        /// Gets current window width.
        /// </summary>
        int WindowWidth { get; }
        
        /// <summary>
        /// Gets current window height.
        /// </summary>
        int WindowHeight { get; }

        /// <summary>
        /// Gets current padding in x axis.
        /// </summary>
        int PaddingX { get; }

        /// <summary>
        /// Gets current padding in y axis.
        /// </summary>
        int PaddingY { get; }

        /// <summary>
        /// Gets window visibility.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Sets and Gets window fade time.
        /// </summary>
        int FadeTime { get; set; }

        /// <summary>
        /// Show window for specified period of time defined by <see cref="FadeTime"/>.
        /// </summary>
        void ShowWindow();
        
        /// <summary>
        /// Hides window.
        /// </summary>
        void HideWindow();
    }
}