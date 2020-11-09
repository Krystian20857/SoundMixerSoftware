using GameOverlay.Drawing;
using GameOverlay.Windows;
using SoundMixerSoftware.Interop.Wrapper;
using SoundMixerSoftware.Overlay.Resource;
using SoundMixerSoftware.Overlay.Utils;

namespace SoundMixerSoftware.Overlay.OverlayWindow
{
    public class CenterTextOverlay : AbstractOverlayWindow
    {
        #region Constant

        public const float FONT_SIZE = 72.0F;
        public const float TEXT_X_MARGIN = 40;
        public const float TEXT_Y_MARGIN = 20;
        public const float BACKGROUND_ROUND = 0;
        
        #endregion
        
        #region Private Fields
        
        private ColorResource _colorResource = new ColorResource();
        private BrushResource _brushResource = new BrushResource();
        private FontResource _fontResource = new FontResource();
        
        #endregion
        
        #region Public Properties

        public string Text { get; set; }
        public float FontSize { get; set; } = FONT_SIZE;

        #endregion
        
        public CenterTextOverlay(int showTime) : base(WindowWrapper.GetScreenWidth(), WindowWrapper.GetScreenHeight(), 0, 0, showTime)
        {
            
        }

        protected override void RenderScene(DrawGraphicsEventArgs args)
        {
            var graphics = args.Graphics;
            var ycenter = graphics.Height / 2;
            var xcenter = graphics.Width / 2;

            
            graphics.ClearScene(Color.Transparent);

            var font = _fontResource["Default"];
            var textSize = graphics.MeasureString(font, FontSize, Text);
            var textX = xcenter - textSize.X / 2;
            var textY = ycenter - textSize.Y / 2;
            
            graphics.FillRoundedRectangle(_brushResource["DarkGray"], textX - TEXT_X_MARGIN, textY - TEXT_Y_MARGIN, xcenter + textSize.X / 2 + TEXT_X_MARGIN, ycenter + textSize.Y / 2 + TEXT_Y_MARGIN, BACKGROUND_ROUND);
            graphics.DrawText(font, FontSize, _brushResource["DimWhite"], textX, textY, Text);
        }

        protected override void CreateResource(SetupGraphicsEventArgs args)
        {
            var graphics = args.Graphics;
            
            Util.CreateBrushes(graphics, _colorResource, _brushResource);
            
            _fontResource.SetResource("Default", graphics.CreateFont("Segoe UI font", 14, bold: true));
        }
    }
}