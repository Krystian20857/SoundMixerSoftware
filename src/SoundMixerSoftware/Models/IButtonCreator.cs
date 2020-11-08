﻿using System.Windows.Media;

namespace SoundMixerSoftware.Models
{
    public interface IButtonCreator
    {
        string Name { get; set; }
        string Key { get; set; }
        ImageSource Image { get; set; }
        int Index { get; set; }
    }
}