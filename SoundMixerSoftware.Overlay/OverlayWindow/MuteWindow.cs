using GameOverlay.Drawing;
using GameOverlay.Windows;
using SoundMixerSoftware.Overlay.Resource;

namespace SoundMixerSoftware.Overlay.OverlayWindow
{
    public class MuteWindow : AbstractOverlayWindow
    {
        #region Consts

        public const int WINDOW_WIDTH = 75;
        public const int WINDOW_HEIGHT = 75;

        public const int IMAGE_WIDTH = 50;
        public const int IMAGE_HEIGHT = 50;
        
        #endregion
        
        #region Private Fields
        
        private IResourceProvider<Color> _colorResource = new ColorResource();
        private IResourceProvider<IBrush> _brushResource = new BrushResource();
        private IResourceProvider<Image> _imageResource = new ImageResource();

        #endregion
        
        #region Public Properties

        public bool IsMuted { get; set; }

        #endregion
        
        #region Constructor
        
        public MuteWindow(int fadeTime) : base(WINDOW_WIDTH, WINDOW_HEIGHT, 25, 25, fadeTime)
        {
            
        }
        
        #endregion

        protected override void RenderScene(DrawGraphicsEventArgs args)
        {
            var graphics = args.Graphics;

            float xcenter = WINDOW_WIDTH / 2;
            float ycenter = WINDOW_HEIGHT / 2;
            
            graphics.ClearScene(Color.Transparent);
            graphics.FillRoundedRectangle(_brushResource["DarkGray"], 0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, 5);
            var image = IsMuted ? _imageResource["SpeakerMute"] : _imageResource["SpeakerUnMute"];
            graphics.DrawImage(image, xcenter - (IMAGE_WIDTH / 2), ycenter - (IMAGE_HEIGHT / 2), xcenter + (IMAGE_WIDTH / 2), ycenter + (IMAGE_HEIGHT / 2));
        }

        protected override void CreateResource(SetupGraphicsEventArgs args)
        {
            var graphics = args.Graphics;

            foreach (var color in _colorResource.GetResources())
                _brushResource.SetResource(color.Key, graphics.CreateSolidBrush(color.Value));
            FontResource.CreateFonts(graphics);
            ImageResource.CreateImages(graphics);
        }
    }
}