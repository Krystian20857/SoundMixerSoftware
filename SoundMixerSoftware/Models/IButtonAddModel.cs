namespace SoundMixerSoftware.Models
{
    public interface IButtonAddModel
    {
        /// <summary>
        /// Defines button add tab name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Occurs when add button has clicked.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool AddClicked(int index);
    }
}