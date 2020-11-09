using System;
using SoundMixerSoftware.Framework.Config;
using SoundMixerSoftware.Interop.Threading;

namespace SoundMixerSoftware.Framework.Threading
{
    public class ProcessWatcher
    {
        /// <summary>
        /// Default process watcher.
        /// </summary>
        public static IProcessWatcher DefaultProcessWatcher { get; } 
            = new Interop.Threading.ProcessWatcher(TimeSpan.FromMilliseconds(ConfigHandler.ConfigStruct.Interop.WatcherWait));
    }
}