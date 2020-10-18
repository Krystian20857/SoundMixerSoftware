using System.Collections.Generic;
using System.Linq;
using GameOverlay.Drawing;

namespace SoundMixerSoftware.Overlay.Resource
{
    public class FontResource : IResourceProvider<Font>
    {

        #region Static Fields
        
        public static readonly IDictionary<string, Font> Fonts = new Dictionary<string, Font>();

        #endregion
        
        #region Static Properties

        public static bool IsInitialized { get; private set; }

        #endregion
        
        #region Static Methods

        public static void CreateFonts(Graphics graphics)
        {
            if(IsInitialized)
                return;
            Fonts.Add("Default", graphics.CreateFont("Segoe UI font", 14, bold: true));
            IsInitialized = true;
        }

        #endregion

        #region Implemented Methods
        
        public Font GetResource(string resourceKey)
        {
            return Fonts.ContainsKey(resourceKey) ? Fonts[resourceKey] : null;
        }

        public void SetResource(string resourceKey, Font resource)
        {
            if (Fonts.ContainsKey(resourceKey))
                Fonts[resourceKey] = resource;
            else
                Fonts.Add(resourceKey, resource);
        }

        public Font this[string resourceKey]
        {
            get => GetResource(resourceKey);
            set => SetResource(resourceKey, value);
        }

        public IEnumerable<KeyValuePair<string, Font>> GetResources()
        {
            return Fonts.ToList();
        }

        #endregion
    }
}