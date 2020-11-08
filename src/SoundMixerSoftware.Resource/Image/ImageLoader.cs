using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Resource.Image
{
    public class ImageLoader
    {
        #region Static

        public static readonly List<string> ImageExtensions = new List<string>()
        {
            ".png",
            ".jpg",
            ".ico",
            ".bmp"
        };
        
        public const string FILE_PREFIX = "file:";
        public const string EMBED_PREFIX = "embed:";
        
        #endregion
        
        #region Private Fields
        
        // build-in thread-safety
        private readonly Hashtable _resources = new Hashtable();
        
        #endregion 
        
        #region Public Properties

        public string WorkingPath { get; set; }
        public ImageSource DefaultImage { get; }

        #endregion
        
        #region Constructor

        public ImageLoader(string workingPath, string defaultImage = "File")
        {
            WorkingPath = workingPath;
            if(!IsDirectory(workingPath))
                throw new InvalidEnumArgumentException($"{nameof(workingPath)} must be directory");
            DefaultImage = LoadFromFile(defaultImage);
        }
        
        #endregion
        
        #region Methods

        public ImageSource GetOrLoad(string name)
        {
            var resource = (ImageSource)_resources[name];

            if (resource != null) return resource;
            
            if (name.StartsWith(FILE_PREFIX))
            {
                resource = LoadFromFile(name.Substring(FILE_PREFIX.Length));
            }
            else if (name.StartsWith(EMBED_PREFIX))
            {
                resource = LoadFromEmbedResource(name.Substring(EMBED_PREFIX.Length));
            }

            return resource;
        }

        private ImageSource LoadFromEmbedResource(string path)
        {
            var handle = IconExtractor.ExtractFromIndex(path).Handle;
            var source = Imaging.CreateBitmapSourceFromHIcon(handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Gdi32.DeleteObject(handle);
            return source;
        }

        private ImageSource LoadFromFile(string fileName)
        {
            var imageFile = MatchFiles(fileName, ImageExtensions).FirstOrDefault();
            if (imageFile == null)
                return null;
            return new BitmapImage(new Uri(imageFile));
        }

        private IEnumerable<string> MatchFiles(string name, IEnumerable<string> extensions)
        {
            foreach (var file in Directory.GetFiles(WorkingPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var extension = Path.GetExtension(file);
                
                if(!fileName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (extensions.Any(x => extension == x))
                    yield return file;
            }
        }

        private static bool IsDirectory(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }
        
        #endregion
    }
}