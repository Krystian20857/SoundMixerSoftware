using GameOverlay.Drawing;
using GameOverlay.Windows;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Overlay.Resource;
using SoundMixerSoftware.Overlay.Utils;
using SoundMixerSoftware.Resource.Image;

namespace SoundMixerSoftware.Overlay.OverlayWindow
{
    public class ProfileOverlay : AbstractOverlayWindow
    {
        #region Consts

        public const int WINDOW_WIDTH = 225;
        public const int WINDOW_HEIGHT = 100;

        public const int IMAGE_WIDTH = 75;
        public const int IMAGE_HEIGHT = 75;

        public const float IMAGE_PADDING_Y = (WINDOW_HEIGHT - IMAGE_HEIGHT) / 2.0F;
        public const float IMAGE_PADDING_X = IMAGE_PADDING_Y;

        public const float CENTER_Y = WINDOW_HEIGHT / 2.0F;
        public const float CENTER_X = WINDOW_WIDTH / 2.0F;

        public const float TEXT_X_PADDING = 5;
        public const float TEXT_CORNER_Y = (WINDOW_HEIGHT - CENTER_Y) / 4.0F; 
        public const float TEXT_CORNER_X = IMAGE_WIDTH + IMAGE_PADDING_X + TEXT_X_PADDING;

        public const float FONT_SIZE = 20;
        public const float MAX_FONT_WIDTH = WINDOW_WIDTH - TEXT_CORNER_X - TEXT_X_PADDING;

        #endregion
        
        #region Private Fields
        
        private IResourceProvider<Color> _colorResource = new ColorResource();
        private IResourceProvider<IBrush> _brushResource = new BrushResource();
        private IResourceProvider<Image> _imageResource = new ImageResource();
        private IResourceProvider<Font> _fontResource = new FontResource();

        private string _profileName;

        private volatile bool _stringCut;
        
        #endregion
        
        #region Public Properties

        
        public string DisplayName
        {
            get => _profileName;
            set
            {
                _profileName = value;
                _stringCut = true;
            }
        }
        

        #endregion
        
        #region Constructor
        
        public ProfileOverlay(int fadeTime) : base(WINDOW_WIDTH, WINDOW_HEIGHT, 25, 25, fadeTime)
        {
        }
        
        #endregion
        
        #region Overridden Methods

        protected override void RenderScene(DrawGraphicsEventArgs args)
        {
            Graphics.BeginScene();
            if (_stringCut)
            {
                _profileName = Util.CutString(Graphics, _fontResource["Default"], FONT_SIZE, _profileName, MAX_FONT_WIDTH);
                _stringCut = false;
            }

            Graphics.ClearScene(Color.Transparent);
            Graphics.FillRoundedRectangle(_brushResource["DarkGray"], 0, 0, WINDOW_WIDTH, WINDOW_HEIGHT, 0);
            Graphics.DrawImage(_imageResource["ProfileImage"], IMAGE_PADDING_X, IMAGE_PADDING_Y, IMAGE_PADDING_X + IMAGE_WIDTH, IMAGE_PADDING_Y + IMAGE_HEIGHT);
            Graphics.DrawText(_fontResource["Default"], FONT_SIZE, _brushResource["DimWhite"], TEXT_CORNER_X, TEXT_CORNER_Y, _profileName);
            Graphics.EndScene();
        }

        protected override void CreateResource(SetupGraphicsEventArgs args)
        {
            var graphics = args.Graphics;
            Util.CreateBrushes(graphics, _colorResource, _brushResource);
            _fontResource.SetResource("Default", graphics.CreateFont("Segoe UI font", 14, bold: true));
            _imageResource.SetResource("ProfileImage", new Image(graphics, Images.Profile.ToByteArray()));
        }
        
        #endregion
    }
}