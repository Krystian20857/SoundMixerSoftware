using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Config.Yaml;
using SoundMixerSoftware.Helpers.AudioSessions;

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
        public List<Session> Applications { get; set; } = new List<Session>();
        public int Index { get; set; }
    }

    public class Session
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public SessionMode SessionMode { get; set; }
        public DataFlow DataFlow { get; set; }
    }
    
    public class ButtonStruct
    {
        public string Function { get; set; }
        public int Index { get; set; }
    }
}