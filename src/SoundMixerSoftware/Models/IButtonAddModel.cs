using System;
using MaterialDesignThemes.Wpf;

namespace SoundMixerSoftware.Models
{
    public interface IButtonAddModel
    {
        /// <summary>
        /// Defines button add tab name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Icon for function
        /// </summary>
        PackIconKind Icon { get; set; }
        /// <summary>
        /// Unique id for faster tab switching
        /// </summary>
        Guid UUID { get; set; }
        /// <summary>
        /// Occurs when add button has clicked.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool AddClicked(int index);
    }
}