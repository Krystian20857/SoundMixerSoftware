using System;
using System.Collections.Generic;
using System.Linq;
using SoundMixerSoftware.Common.Property;
using SoundMixerSoftware.Overlay.OverlayWindow;

namespace SoundMixerSoftware.Helpers.Overlay
{
    public class OverlaySwitcher : IProperties<string, IOverlay>
    {
        
        #region Private Fields
        
        private Dictionary<string, IOverlay> _overlays = new Dictionary<string, IOverlay>();
        
        #endregion
        
        #region Public Properties

        public Func<bool> EnableFunc { get; set; }

        #endregion

        #region Constructor
       
        public OverlaySwitcher(Func<bool> enableFunc)
        {
            EnableFunc = enableFunc;
        }
        
        #endregion
        
        #region Implemented Methods

        public IOverlay GetValue(string key)
        {
            if (!_overlays.ContainsKey(key))
                return default;
            return _overlays[key];
        }

        public void SetValue(string key, IOverlay value)
        {
            if (_overlays.ContainsKey(key))
            {
                _overlays[key] = value;
                return;
            }
            
            _overlays.Add(key, value);
        }

        public bool RemoveValue(string key)
        {
            return _overlays.Remove(key);
        }

        public IEnumerable<IOverlay> GetValues()
        {
            return _overlays.Values;
        }

        public IEnumerable<string> GetKeys()
        {
            return _overlays.Keys;
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Show overlay and hides other.
        /// </summary>
        /// <param name="valueChange"></param>
        /// <typeparam name="T"></typeparam>
        public void HandleOverlay<T>(Action<string, T> valueChange)
        {
            if (!EnableFunc.Invoke())
                return;
            foreach (var pair in _overlays)
            {
                var key = pair.Key;
                var overlay = pair.Value;
                
                if (overlay is T genericOverlay)
                {
                    valueChange(key, genericOverlay);
                    overlay.ShowWindow();
                }
                else
                    overlay.HideWindow();
            }
        }
        
        #endregion
    }
}