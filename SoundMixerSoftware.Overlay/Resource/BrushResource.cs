using System.Collections.Generic;
using System.Linq;
using GameOverlay.Drawing;

namespace SoundMixerSoftware.Overlay.Resource
{
    public class BrushResource : IResourceProvider<IBrush>
    {
        #region Fields
        
        private Dictionary<string, IBrush> Brushes = new Dictionary<string, IBrush>();
        
        #endregion

        #region Implemented Methods
        
        public IBrush GetResource(string resourceKey)
        {
            return Brushes.ContainsKey(resourceKey) ? Brushes[resourceKey] : null;
        }

        public void SetResource(string resourceKey, IBrush resource)
        {
            if (Brushes.ContainsKey(resourceKey))
                Brushes[resourceKey] = resource;
            else
                Brushes.Add(resourceKey, resource);
        }

        public IBrush this[string resourceKey]
        {
            get => GetResource(resourceKey);
            set => SetResource(resourceKey, value);
        }

        public IEnumerable<KeyValuePair<string, IBrush>> GetResources()
        {
            return Brushes.ToList();
        }
        
        #endregion
    }
}