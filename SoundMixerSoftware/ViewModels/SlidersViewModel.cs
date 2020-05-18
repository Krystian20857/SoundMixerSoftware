using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using SoundMixerSoftware.Models;
using XamlReader = System.Windows.Markup.XamlReader;

namespace SoundMixerSoftware.ViewModels
{
    public class SlidersViewModel : ITabModel
    {
        #region Private Fields
        
        private BindableCollection<SliderModel> _sliders = new BindableCollection<SliderModel>();
        
        #endregion
        
        #region Public Properties

        public BindableCollection<SliderModel> Sliders
        {
            get => _sliders;
            set
            {
                _sliders = value;
            }
        }
        
        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }

        #endregion
        

        #region Constructor
        
        public SlidersViewModel()
        {
            Name = "Sliders";
            Icon = PackIconKind.VolumeSource;
            
            for(var n = 0; n < 2; n++)
            Sliders.Add(new SliderModel()
            {
                Volume = 55,
                Applications = new BindableCollection<AppModel>(CreateSliderModel())
            });
        }

        private IEnumerable<AppModel> CreateSliderModel()
        {
            var directory = @"E:\other\icons\24x24 icon pack\png\24x24";
            var files = Directory.GetFiles(directory);
            for (var n = 0; n < files.Length; n++)
            {
                var file = files[n];
                yield return new AppModel
                {
                    App = Path.GetFileName(file),
                    Image = new BitmapImage(new Uri(file))
                };
                if (n == 15)
                    yield break;
            }
        }
        
        #endregion
    }
}