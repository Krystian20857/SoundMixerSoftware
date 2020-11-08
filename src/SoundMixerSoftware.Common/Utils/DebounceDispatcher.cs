using System;
using System.Windows.Threading;

namespace SoundMixerSoftware.Common.Utils
{
    public class DebounceDispatcher
    {
       #region Private Fields

       private DispatcherTimer _timer;

       #endregion
       
       #region Public Methods

       public void Debounce(int interval, Action<object> action, object param = null, DispatcherPriority priority = DispatcherPriority.ApplicationIdle)
       {
           _timer?.Stop();
           _timer = null;
           _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(interval), priority, (sender, args) =>
           {
               if (_timer == null)
                   return;
               _timer?.Stop();
               _timer = null;
               action.Invoke(param);
           }, Dispatcher.CurrentDispatcher );
       }

       #endregion
    }
}