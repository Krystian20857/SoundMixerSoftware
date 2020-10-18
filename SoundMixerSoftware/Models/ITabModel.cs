using System;
using MaterialDesignThemes.Wpf;

namespace SoundMixerSoftware.Models
{
    /// <summary>
    /// Model used in main window tabcontrol.
    /// </summary>
    public interface ITabModel
    {
        /// <summary>
        /// Name of the page.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Material Design Icon.
        /// </summary>
        PackIconKind Icon { get; set; }
        /// <summary>
        /// Represents unique id of tab.
        /// </summary>
        Guid Uuid { get; set; }
    }
}