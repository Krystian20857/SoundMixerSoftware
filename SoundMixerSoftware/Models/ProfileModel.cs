namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Model of profile 
    /// </summary>
    public class ProfileModel
    {
        /// <summary>
        /// Profile Name
        /// </summary>
        public string ProfileName { get; set; }
        /// <summary>
        /// Sliders attached to profile.
        /// </summary>
        public int SliderCount { get; set; }
        /// <summary>
        /// Buttons attached to profile.
        /// </summary>
        public int ButtonCount { get; set; }
    }
}