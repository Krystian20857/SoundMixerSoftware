using System.Collections.Generic;
using System.Linq;
using GameOverlay.Drawing;
using SoundMixerSoftware.Common.Extension;

namespace SoundMixerSoftware.Overlay.Resource
{
    public class ImageResource : IResourceProvider<Image>
    {
        #region Fields
        
        public static readonly Dictionary<string, Image> Images = new Dictionary<string, Image>();
        
        #endregion

        #region Implemented Methods
        
        public Image GetResource(string resourceKey)
        {
            return Images.ContainsKey(resourceKey) ? Images[resourceKey] : null;
        }

        public void SetResource(string resourceKey, Image resource)
        {
            if (Images.ContainsKey(resourceKey))
                Images[resourceKey] = resource;
            else
                Images.Add(resourceKey, resource);
        }

        public Image this[string resourceKey]
        {
            get => GetResource(resourceKey);
            set => SetResource(resourceKey, value);
        }

        public IEnumerable<KeyValuePair<string, Image>> GetResources()
        {
            return Images.ToList();
        }
        
        #endregion
    }
}