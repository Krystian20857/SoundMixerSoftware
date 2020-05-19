﻿using System.Windows.Controls;
using Caliburn.Micro;
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
    }
}