using System;
using System.Threading;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using SoundMixerSoftware.Interop.Wrapper;
using Timer = System.Timers.Timer;

namespace SoundMixerSoftware.Overlay.OverlayWindow
{
    public abstract class AbstractOverlayWindow : IOverlay, IDisposable
    {
        #region Private Fields

        private Timer _showTimer;
        private volatile bool _fadeThreadRunning = true;
        private int _backupFadeTime;
        
        private readonly object _showLock = new object();

        #endregion

        #region Protected Fields

        protected volatile GraphicsWindow _window;
        protected volatile byte _opacity = 255;

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
            get => (int) _showTimer.Interval;
            set => _showTimer.Interval = value;
        }

        public int TempFadeTime
        {
            get => IsTempFadeTime ? FadeTime : -1;
            set
            {
                _backupFadeTime = FadeTime;
                IsTempFadeTime = true;
                FadeTime = value;
            }
        }

        public bool IsTempFadeTime { get; set; }

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

            WindowWrapper.ApplyOpacityFlag(_window.Handle);

            SetShowTimer(fadeTime);
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

        private void SetShowTimer(int showTime)
        {
            _showTimer = new Timer { Interval = showTime };
            _showTimer.Elapsed += (sender, args) =>
            {
                var fadeTickTime = (int) (showTime * 0.1F) / 50;
                _fadeThreadRunning = true;
                while (_opacity > 5 && _fadeThreadRunning)
                {
                    WindowWrapper.SetWindowOpacity(_window.Handle, _opacity);
                    _opacity--;
                    Thread.Sleep(fadeTickTime);
                }

                if (_fadeThreadRunning)
                    HideWindow();
                _opacity = 255;
                WindowWrapper.SetWindowOpacity(_window.Handle, _opacity);
            };
        }

        #endregion

        #region Public Methods
        
        public void ShowWindow()
        {
            lock (_showLock)
            {
                _opacity = 255;
                _fadeThreadRunning = false;
                if (!_window.IsInitialized)
                    _window.Create();
                if (!IsVisible)
                {
                    _window.Show();
                    _window.Unpause();
                }

                _showTimer.Start();
                WindowWrapper.SetWindowPos(_window.Handle, PaddingX, PaddingY);
            }
        }

        public void HideWindow()
        {
            lock (_showLock)
            {
                if (!IsVisible)
                    return;
                _window.Pause();
                _window.Hide();
                _opacity = 0;
                _fadeThreadRunning = false;
                _showTimer.Stop();
                if (IsTempFadeTime)
                {
                    IsTempFadeTime = false;
                    FadeTime = _backupFadeTime;
                    _backupFadeTime = 0;
                }
            }
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