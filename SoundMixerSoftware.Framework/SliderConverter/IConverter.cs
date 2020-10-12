using System;
using System.Collections.Generic;
using System.Windows.Media;
using SoundMixerSoftware.Helpers.Device;

namespace SoundMixerSoftware.Helpers.SliderConverter
{
    public interface IConverter
    {
        /// <summary>
        /// Gets and sets name of converter.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets and sets key of converter.
        /// </summary>
        string Key { get; set; }
        /// <summary>
        /// Gets and sets index of slider index of current converter.
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// Gets and sets unique id od converter.
        /// </summary>
        Guid UUID { get; set; }
        /// <summary>
        /// Save current converter to container.
        /// </summary>
        /// <returns></returns>
        Dictionary<object, object> Save();
        /// <summary>
        /// Convert input value.
        /// </summary>
        /// <param name="inputValue">Input value</param>
        /// <param name="sliderIndex">Slider index</param>
        /// <param name="device">Device id</param>
        /// <returns>Converted output.</returns>
        float Convert(float inputValue, int sliderIndex,int converterIndex, DeviceId device);
    }
}