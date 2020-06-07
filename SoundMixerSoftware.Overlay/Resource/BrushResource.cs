using System.Collections.Generic;
using System.Linq;
using GameOverlay.Drawing;
using SharpDX.Direct2D1;

namespace SoundMixerSoftware.Overlay.Resource
{
    public class BrushResource : IResourceProvider<IBrush>
    {
        #region Static Fields
        
        private static Dictionary<string, IBrush> Brushes = new Dictionary<string, IBrush>();
        
        #endregion
        
        #region Static Properties

        /// <summary>
        /// Gets state of brushes.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion
        
        #region Static Methods

        /// <summary>
        /// Create brushes dictionary from colors resources.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="colors">Resource dictionary.</param>
        public static void CreateFromColors(Graphics graphics, IResourceProvider<Color> colors)
        {
            if(IsInitialized)
                return;
            foreach (var keyPair in colors.GetResources())
                AddBrush(graphics, keyPair.Key, keyPair.Value);

            IsInitialized = true;
        }

        /// <summary>
        /// Add brush using graphics.
        /// </summary>
        /// <param name="graphics">Graphics.</param>
        /// <param name="resourceKey">Key to brush.</param>
        /// <param name="color">Brush value.</param>
        public static void AddBrush(Graphics graphics, string resourceKey, Color color)
        {
            Brushes.Add(resourceKey, graphics.CreateSolidBrush(color));
        }

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