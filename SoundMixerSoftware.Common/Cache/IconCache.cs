using System;
using System.Drawing;
using System.IO;
using NLog;

namespace SoundMixerSoftware.Common.Cache
{
    public class IconCache : ICache<Icon>
    {
        #region Private Methods

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Folder path where cache is stored.
        /// </summary>
        public string FolderPath { get; }

        #endregion
        
        #region Constructor

        public IconCache(string directory)
        {
            FolderPath = directory;
        }
        
        #endregion

        #region Implemented Methods
        
        public Icon GetOrCreate(object key, Func<Icon> createItem)
        {
            var filePath = Path.Combine(FolderPath, key.ToString());
            if (!File.Exists(filePath))
            {
                var icon = createItem.Invoke();
                icon.ToBitmap().Save(filePath);
                return icon;
            }
            using (var bitmap = new Bitmap(filePath, true))
                return Icon.FromHandle(bitmap.GetHicon());
        }
        
        #endregion
    }
}