using SoundMixerSoftware.Helpers.Buttons.Functions;

namespace SoundMixerSoftware.Models
{
    public class MediaFunctionModel
    {
        /// <summary>
        /// Defines visible name;
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Define MediaTask linked name;
        /// </summary>
        public MediaTask MediaTask { get; set; }
    }
}