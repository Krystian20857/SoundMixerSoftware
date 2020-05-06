using System;
using System.Drawing;
using NLog;

namespace SoundMixerAppv2.Common.Cache
{
    public class IconCache : ICache<Icon>
    {
        #region Logger

        /// <summary>
        /// Current Class Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Implemented Methods
        
        public Icon GetOrCreate(object key, Func<Icon> createItem)
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}