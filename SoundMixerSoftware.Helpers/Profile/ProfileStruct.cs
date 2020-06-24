using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Buttons;
using YamlDotNet.Serialization;

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

        public string Name { get; set; }
    }

    public class Session
    {
        public string Name { get; set; }
        public string ID { get; set; }
        
        [YamlIgnore]
        public SessionMode SessionMode { get; set; }
        
        [YamlMember(Alias = "SessionMode")]
        public string SessionModeString
        {
            get => SessionMode.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                    SessionMode = default;
                else
                    SessionMode = Enum.TryParse<SessionMode>(value, out var result) ? result : default;
            }
        }
        
        [YamlIgnore]
        public DataFlow DataFlow { get; set; }
        
        [YamlMember(Alias = "DataFlow")]
        public string DataFlowString
        {
            get => DataFlow.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                    DataFlow = default;
                else
                    DataFlow = Enum.TryParse<DataFlow>(value, out var result) ? result : default;
            }
        }
    }
    
    public class ButtonStruct
    {
        [YamlIgnore]
        public ButtonFunction Function { get; set; }

        [YamlMember(Alias = "Function")]
        public string FunctionString
        {
            get => Function.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                    Function = default;
                else
                    Function = Enum.TryParse<ButtonFunction>(value, out var result) ? result : default;
            }
        }

        public string Name { get; set; }
    }
}