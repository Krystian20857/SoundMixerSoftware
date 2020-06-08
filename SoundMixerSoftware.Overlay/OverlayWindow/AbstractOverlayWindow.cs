using System;
using System.Timers;
using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace SoundMixerSoftware.Overlay.OverlayWindow
{
    public abstract class AbstractOverlayWindow : IDisposable
    {
        #region Private Fields

        private Timer _fadeTimer;

        #endregion
        
        #region Protected Fields

        protected volatile GraphicsWindow _window;
        
        #endregion
        
        #region Protected Properties

        protected Graphics Graphics => _window.Graphics;

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets current window width.
        /// </summary>
        public int WindowWidth { get; }
        /// <summary>
        /// Gets current window height.
        /// </summary>
        public int WindowHeight { get; }
        /// <summary>
        /// Gets current padding in x axis.
        /// </summary>
        public int PaddingX { get; }
        /// <summary>
        /// Gets current padding in y axis.
        /// </summary>
        public int PaddingY { get; }

        public bool IsVisible => _window.IsVisible && _window.IsRunning;

        public int FadeTime
        {
            get => (int) _fadeTimer.Interval;
            set => _fadeTimer.Interval = value;
        }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Create overlay instance and setup graphics.
        /// </summary>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <param name="paddingX"></param>
        /// <param name="paddingY"></param>
        /// <param name="fadeTime"></param>
        protected AbstractOverlayWindow(int windowWidth, int windowHeight, int paddingX, int paddingY, int fadeTime)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            PaddingX = paddingX;
            PaddingY = paddingY;
            
            var graphics = new Graphics
            {
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                VSync = true
            };

            _window = new GraphicsWindow(PaddingX, PaddingY, WindowWidth, WindowHeight, graphics)
            {
                IsTopmost = true,
                IsVisible = true,
            };
            
            _window.DestroyGraphics += WindowOnDestroyGraphics;
            _window.DrawGraphics += WindowOnDrawGraphics;
            _window.SetupGraphics += WindowOnSetupGraphics;

            _fadeTimer = SetupFadeTimer(fadeTime);
            FadeTime = fadeTime;
        }
        
        #region Private Events

        private void WindowOnDrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            RenderScene(e);
        }

        private void WindowOnDestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            _window.Dispose();
        }

        private void WindowOnSetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            CreateResource(e);
        }

        #endregion

        #endregion
        
        #region Private Methods

        private Timer SetupFadeTimer(int fadeTime)
        {
            var fadeTimer = new Timer
            {
                Interval = fadeTime,
            };
            fadeTimer.Elapsed += (sender, args) =>
            {
                HideWindow();
            };
            return fadeTimer;
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Show window for specified period of time defined by <see cref="FadeTime"/>.
        /// </summary>
        public void ShowWindow()
        {
            if(IsVisible)
                return;
            if(!_window.IsInitialized)
                _window.Create();
            _window.Show();
            _window.Unpause();
            if (_fadeTimer.Enabled)
                _fadeTimer.Stop();
            _fadeTimer.Start();
        }

        public void HideWindow()
        {
            if(!IsVisible)
                return;
            _fadeTimer.Start();
            _window.Pause(); 
            _window.Hide();
        }
        
        #endregion
        
        #region Abstract Methods

        /// <summary>
        /// Render scene.
        /// </summary>
        /// <param name="args"></param>
        protected abstract void RenderScene(DrawGraphicsEventArgs args);
        /// <summary>
        /// Create resources.
        /// </summary>
        /// <param name="args"></param>
        protected abstract void CreateResource(SetupGraphicsEventArgs args);

        #endregion
        
        #region Dispose

        public void Dispose()
        {
            _window?.Dispose();
        }
        
        #endregion
    }
}