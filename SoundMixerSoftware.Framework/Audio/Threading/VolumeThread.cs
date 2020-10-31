using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoundMixerSoftware.Framework.Audio.Threading
{
    public static class VolumeThread
    {
        
        #region Fields
        
        private static VolumeScheduler _scheduler = new VolumeScheduler();
        
        #endregion
        
        #region Properties

        public static bool RequireInvoke => _scheduler.RequireInvoke;
        
        #endregion
        
        #region Public Methods

        public static void Invoke(Action action)
        {
            if (!RequireInvoke)
                action();
            else
                BeginInvoke(action);
        }

        public static T Invoke<T>(Func<T> func)
        {
            return RequireInvoke ? BeginInvoke(func) : func();
        }

        public static void BeginInvoke(Action action)
        {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _scheduler);
        }

        public static T BeginInvoke<T>(Func<T> func)
        {
            return Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, _scheduler).GetAwaiter().GetResult();
        }

        #endregion
        
    }
}