using System;
using System.ComponentModel;
using System.Windows.Controls;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.ViewModels
{
    public class ManagerViewModel : ITabModel
    {
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        public ManagerViewModel()
        {
            Name = "Profiles ELo320";
            Icon = PackIconKind.Abc;
        }
    }
}