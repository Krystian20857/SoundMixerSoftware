using System;
using SoundMixerSoftware.Common.Threading;
using SoundMixerSoftware.Helpers.Config;

namespace SoundMixerSoftware.Helpers.Threading
{
    public class ProcessWatcher
    {
        /// <summary>
        /// Default process watcher.
        /// </summary>
        public static IProcessWatcher DefaultProcessWatcher { get; } 
            = new Win32.Threading.ProcessWatcher(TimeSpan.FromMilliseconds(ConfigHandler.ConfigStruct.Interop.WatcherWait));
    }
}