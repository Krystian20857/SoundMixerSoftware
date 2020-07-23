using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoundMixerSoftware.Common.Threading.Com
{
    /// <summary>
    /// Based on: https://github.com/Belphemur/SoundSwitch/blob/dev/SoundSwitch.Audio.Manager/Interop/Com/Threading/ComThread.cs
    /// </summary>
    public static class ComThread
    {
        #region Private fields
        
        private static ComTaskScheduler _scheduler = new ComTaskScheduler();
        
        #endregion
        
        #region Public Properties

        public static bool RequireInvoke => Thread.CurrentThread.ManagedThreadId != _scheduler.ThreadId;

        #endregion
        
        #region Public Methods

        public static void Invoke(Action action)
        {
            if (RequireInvoke)
                BeginInvoke(action).Wait();
            else
                action();
        }

        public static Task BeginInvoke(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _scheduler);
        }

        public static T Invoke<T>(Func<T> func)
        {
            return RequireInvoke ? BeginInvoke(func).GetAwaiter().GetResult() : func();
        }

        public static Task<T> BeginInvoke<T>(Func<T> func)
        {
            return Task<T>.Factory.StartNew(func,CancellationToken.None, TaskCreationOptions.None, _scheduler);
        }
        
        #endregion
    }
}