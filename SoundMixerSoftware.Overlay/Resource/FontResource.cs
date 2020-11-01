using System.Collections.Generic;
using System.Linq;
using GameOverlay.Drawing;

namespace SoundMixerSoftware.Overlay.Resource
{
    public class FontResource : IResourceProvider<Font>
    {

        #region Fields
        
        public readonly IDictionary<string, Font> Fonts = new Dictionary<string, Font>();

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