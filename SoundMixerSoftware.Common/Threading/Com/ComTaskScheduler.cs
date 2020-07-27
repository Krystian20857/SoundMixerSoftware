using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundMixerSoftware.Common.Threading.Com
{
    /// <summary>
    /// Based on: https://github.com/Belphemur/SoundSwitch/blob/dev/SoundSwitch.Audio.Manager/Interop/Com/Threading/ComTaskScheduler.cs
    /// </summary>
    public class ComTaskScheduler : TaskScheduler
    {
        #region Private Fields

        private readonly Thread _thread;
        private BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        
        #endregion
        
        #region Public Properties

        public int ThreadId => _thread?.ManagedThreadId ?? -1;
        
        #endregion
        
        #region Constructor

        public ComTaskScheduler()
        {
            _thread = new Thread(() =>
            {
                foreach (var task in _tasks.GetConsumingEnumerable())
                {
                    TryExecuteTask(task);
                }

                Thread.CurrentThread.Join(1);
            }) {Priority = ThreadPriority.Highest, IsBackground = true};
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Name = "ComThread";
            
            _thread.Start();
        }
        
        #endregion
        
        #region Overriden Methods

        public override int MaximumConcurrencyLevel => 1;

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

        protected override IEnumerable<Task> GetScheduledTasks() => _tasks.ToArray();

        #endregion
    }
}