using System;
using System.Diagnostics;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using SharpDX.Direct2D1;
using SoundMixerSoftware.Overlay.Resource;

namespace SoundMixerSoftware.Overlay.OverlayWindow
{
    public class VolumeOverlay : AbstractOverlayWindow
    {
        #region Const

        public const int WINDOW_WIDTH = 55;
        public const int WINDOW_HEIGHT = 125;

        public const int VOLUME_WIDTH = 20;
        public const int VOLUME_HEIGHT = 80;

        public const int VOLUME_OFFSET = 10;
        public const int LABEL_OFFSET = 45;

        public const float FONT_SIZE = 17.0F;

        #endregion
        
        #region Resources
        
        private IResourceProvider<Color> _colorResource = new ColorResource();
        private IResourceProvider<IBrush> _brushResource = new BrushResource();
        private IResourceProvider<Font> _fontResource = new FontResource();

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets and Sets present volume.
        /// </summary>
        public float Volume { get; set; }

        #endregion
        
        #region Constructor
        
        public VolumeOverlay() : base(WINDOW_WIDTH, WINDOW_HEIGHT, 25, 25, 10000)
        {
            ThemeManager.ReloadResources += ThemeManagerOnReloadResources;
        }

        #endregion
        
        #region Private Events
        
        private void ThemeManagerOnReloadResources(object sender, EventArgs e)
        {
            _brushResource["Theme"] =  Graphics.CreateSolidBrush(ThemeManager.ThemeColor);
        }
        
        #endregion
        
        #region Protected Methods
        
        protected override void RenderScene(DrawGraphicsEventArgs args)
        {
            var graphics = args.Graphics;

            float ycenter = WINDOW_HEIGHT / 2;
            float xcenter = WINDOW_WIDTH / 2;
            
            graphics.ClearScene(Color.Transparent);
            graphics.FillRoundedRectangle(_brushResource.GetResource("DarkGray"), 0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, 5);
            graphics.DrawHorizontalProgressBar(_brushResource["LightGray"], _brushResource["Theme"],
                xcenter - VOLUME_WIDTH / 2,
                ycenter - VOLUME_HEIGHT / 2 - VOLUME_OFFSET,
                (float) (xcenter + 0.5*VOLUME_WIDTH),
                (float) (ycenter + 0.5*VOLUME_HEIGHT) - VOLUME_OFFSET,
                0,
                Volume);
            var text = $"{Math.Floor(Volume)}%";
            var textSize = graphics.MeasureString(_fontResource["Default"], text);
            graphics.DrawText(_fontResource["Default"], FONT_SIZE, _brushResource["DimWhite"], xcenter - (textSize.X / 2), ycenter - (textSize.Y / 2) + LABEL_OFFSET, text);
        }

        protected override void CreateResource(SetupGraphicsEventArgs args)
        {
            var graphics = args.Graphics;
            
            BrushResource.CreateFromColors(graphics, _colorResource);
            FontResource.CreateFonts(graphics);
            
            _brushResource["Theme"] = graphics.CreateSolidBrush(ThemeManager.ThemeColor);
        }
        
        #endregion
    }
}