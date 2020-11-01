using System;
using System.Collections.Generic;
using System.Windows.Media;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Framework.Audio;
using SoundMixerSoftware.Framework.Profile;

namespace SoundMixerSoftware.Framework.Buttons.Functions
{
    public class VolumeFunction : IButtonFunction
    {
        #region Constant

        public const string KEY = "vol_func";
        
        public const string VOLUME_KEY = "volume";
        public const string SLIDER_INDEX_KEY = "slider_index";
        
        #endregion
        
        #region Private Fields

        private string _name;
        
        #endregion
        
        #region Public Proeprties

        public int SliderIndex { get; set; }
        public float Volume { get; set; }

        #endregion
        
        #region Implemented Properties

        public string Name
        {
            get
            {
                if (SliderIndex >= ProfileHandler.SelectedProfile.SliderCount)
                    _name = "Slider Volume: slider out of size";
                else
                    _name = $"Slider Volume: {Math.Round(Volume, 2)}% ({ProfileHandler.SelectedProfile.Sliders[SliderIndex].Name})";
                return _name;
            }
            set => _name = value;
        }

        public string Key { get; } = KEY;
        public int Index { get; set; }
        public Guid UUID { get; set; }
        public ImageSource Image { get; set; } = Resource.VolumeIcon.ToImageSource();
        
        #endregion
        
        #region Constrcutor

        public VolumeFunction(Guid uuid, int sliderIndex, float volume)
        {
            UUID = uuid;
            SliderIndex = sliderIndex; 
            Volume = volume;
        }

        public VolumeFunction(int index, Guid uuid, int sliderIndex, float volume) : this(uuid, sliderIndex, volume)
        {
            Index = index;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var container = new Dictionary<object, object>();
            container.Add(SLIDER_INDEX_KEY, SliderIndex);
            container.Add(VOLUME_KEY, Volume);
            return container;
        }

        public void ButtonKeyDown(int index)
        {
            SessionHandler.SetVolume(SliderIndex, Volume, true);
        }

        public void ButtonKeyUp(int index)
        {
            
        }
        
        #endregion
    }

    public class VolumeFunctionCreator : IButtonCreator
    {
        public IButtonFunction CreateButton(int index, Dictionary<object, object> container, Guid uuid)
        {
            var sliderIndexObject = container.ContainsKey(VolumeFunction.SLIDER_INDEX_KEY) ? container[VolumeFunction.SLIDER_INDEX_KEY] : 0;
            var sliderIndex = int.TryParse(sliderIndexObject.ToString(), out var result) ? result : 0;
            
            var volumeObject = container.ContainsKey(VolumeFunction.VOLUME_KEY) ? container[VolumeFunction.VOLUME_KEY] : 0;
            var volume = int.TryParse(volumeObject.ToString(), out var result1) ? result1 : 0; 
            
            return new VolumeFunction(index, uuid, sliderIndex, volume);
        }
    }
}