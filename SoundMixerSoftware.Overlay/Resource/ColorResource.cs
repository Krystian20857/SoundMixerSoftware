using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameOverlay.Drawing;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Overlay.Resource
{
    public class ColorResource : IResourceProvider<Color>
    {
        #region Consts

        public const int TRANSPARENCY = 230;
        
        #endregion
        
        #region Static Fields
        
        public static readonly IDictionary<string, Color> Colors = new Dictionary<string, Color>();

        #endregion
        
        #region Static Constructor

        /// <summary>
        /// Create colors.
        /// </summary>
        static ColorResource()
        {
            Colors.Add("DarkGray", new Color(50,50,50, TRANSPARENCY));
            Colors.Add("LightGray", new Color(200,200,200,TRANSPARENCY));
            Colors.Add("DimWhite", new Color(245, 245, 245, TRANSPARENCY));
        }

        #endregion

        #region Implemented Methods
        
        public Color GetResource(string resourceKey)
        {
            return Colors.ContainsKey(resourceKey) ? Colors[resourceKey] : Color.Transparent;
        }

        public void SetResource(string resourceKey, Color resource)
        {
            if (Colors.ContainsKey(resourceKey))
                Colors[resourceKey] = resource;
            else
                Colors.Add(resourceKey, resource);
        }

        public Color this[string resourceKey]
        {
            get => GetResource(resourceKey);
            set => SetResource(resourceKey, value);
        }

        public IEnumerable<KeyValuePair<string, Color>> GetResources()
        {
            return Colors.ToList();
        }

        #endregion
    }
}