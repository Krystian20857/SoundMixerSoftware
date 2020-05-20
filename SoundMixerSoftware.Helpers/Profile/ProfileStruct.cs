using System.Collections.Generic;

namespace SoundMixerSoftware.Helpers.Profile
{
    public class ProfileStruct
    {
        public string Name { get; set; }
        public int ButtonCount { get; set; }
        public int SliderCount { get; set; }
        public List<string> Applications { get; set; } = new List<string>();
        public List<string> Buttons { get; set; } = new List<string>();
    }
}