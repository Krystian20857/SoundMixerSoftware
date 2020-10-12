using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Helpers.Profile
{
    public class ProfileStruct
    {
        public string Name { get; set; }
        public int ButtonCount { get; set; }
        public int SliderCount { get; set; }
        public List<SliderStruct> Sliders { get; set; } = new List<SliderStruct>();
        public List<ButtonStruct> Buttons { get; set; } = new List<ButtonStruct>();
    }
    
    public class SliderStruct
    {
        public string Name { get; set; }
        public List<Session> Sessions { get; set; } = new List<Session>();
        public List<ConverterStruct> Converters { get; set; } = new List<ConverterStruct>();
    }

    public class Session
    {
        public string Key { get; set; }
        public Dictionary<object, object> Container { get; set; }
        public Guid UUID { get; set; }
    }
    
    public class ButtonStruct
    {
        public string Name { get; set; }
        public List<ButtonFunction> Functions { get; set; } = new List<ButtonFunction>();
    }

    public class ButtonFunction
    {
        public string Key { get; set; }
        public Dictionary<object, object> Container { get; set; }
        public Guid UUID { get; set; }
    }

    public class ConverterStruct
    {
        public string Key { get; set; }
        public Dictionary<object, object> Container { get; set; }
        public Guid UUID { get; set; }
    }
}