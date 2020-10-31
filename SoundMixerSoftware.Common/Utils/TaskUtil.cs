using System;
using System.Windows.Threading;

namespace SoundMixerSoftware.Common.Utils
{
    public class TaskUtil
    {
        private static readonly Dispatcher _dispatcher = System.Windows.Application.Current.Dispatcher;

        public static DispatcherOperation BeginInvokeDispatcher(Action action)
        {
            return _dispatcher.BeginInvoke(action, DispatcherPriority.Send);
        }

        public static T InvokeDispatcher<T>(Func<T> func)
        {
            return _dispatcher.Invoke(func, DispatcherPriority.Send);
        }
    }
}