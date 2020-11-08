using System;
using System.Threading;
using System.Windows.Threading;

namespace SoundMixerSoftware.Common.Utils
{
    public static class DispatcherHelper
    {
        private static Dispatcher _dispatcher = System.Windows.Application.Current.Dispatcher;
        public static bool RequireInvoke => Thread.CurrentThread.ManagedThreadId != _dispatcher.Thread.ManagedThreadId;

        public static void Invoke(Action action)
        {
            if (RequireInvoke)
                _dispatcher.Invoke(action);
            else
                action();
        }
    }
}