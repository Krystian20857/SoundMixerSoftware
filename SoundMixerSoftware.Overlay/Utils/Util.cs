using System;
using System.Linq;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using SharpDX.Direct2D1;
using SoundMixerSoftware.Overlay.Resource;
using LinearGradientBrush = GameOverlay.Drawing.LinearGradientBrush;

namespace SoundMixerSoftware.Overlay.Utils
{
    public static class Util
    {
        public static void CreateBrushes(Graphics graphics ,IResourceProvider<Color> colors, IResourceProvider<IBrush> brushes)
        {
            foreach (var color in colors.GetResources())
                brushes.SetResource(color.Key, graphics.CreateSolidBrush(color.Value));
        }

        public static string CutString(Graphics graphics, Font font, float fontSize, string input, float maxWidth, string suffix = "...")
        {
            var stringWidth = graphics.MeasureString(font, fontSize, input).X;
            if (stringWidth <= maxWidth)
                return input;
            
            var suffixWidth = graphics.MeasureString(font, fontSize, suffix).X;
            while (stringWidth + suffixWidth > maxWidth)
            {
                input = input.Substring(0, input.Length - 1);

                stringWidth = graphics.MeasureString(font, fontSize, input).X;
            }

            input += suffix;
            return input;
        }

        /*
        public static void TestMethod()
        {
            var graphics = new Graphics
            {
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                VSync = true
            };
            
            var _window = new GraphicsWindow(0, 0, 1000, 1000, graphics)
            {
                IsTopmost = true,
                IsVisible = true,
            };
            
            _window.Create();
            
            var input = "abcdefghijk.1231231234567890";
            Font font = null;
            IBrush brush = null;
            
            _window.SetupGraphics += (sender, args) =>
            {
                brush = new LinearGradientBrush(graphics, Color.Blue);
                font = graphics.CreateFont("Segoe UI font", 32, true);
            };
            
            _window.DrawGraphics += (sender, args) =>
            {
                graphics.BeginScene();
                graphics.ClearScene(Color.Transparent);
                graphics.DrawLine(brush, 0,0, 250,0, 5.0F);
                var output = CutString(graphics, font, 32, input, 250);
                var textPoint = graphics.MeasureString(font, 32, output); 
                graphics.DrawLine(brush, 0, textPoint.Y, textPoint.X, textPoint.Y, 5.0F);
                graphics.DrawText(font, 32, brush, 0, 0, output);
                graphics.EndScene();
            };
            
            _window.Join();
        }
        */
        
    }
}