using System;
using SoundMixerSoftware.Common.Threading;
using SoundMixerSoftware.Framework.Config;

namespace SoundMixerSoftware.Framework.Threading
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