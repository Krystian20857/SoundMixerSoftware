using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundMixerSoftware.Framework.Audio.Threading
{
    public class VolumeScheduler : TaskScheduler
    {
        #region Private Fields
        
        private BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private List<Thread> _threads = new List<Thread>();
        private List<int> _threadsIds = new List<int>();

        #endregion
        
        #region Properties

        public override int MaximumConcurrencyLevel => _threads.Count;
        public bool RequireInvoke => !_threadsIds.Contains(Thread.CurrentThread.ManagedThreadId);
        //public 

        #endregion

        #region Constructor
        
        public VolumeScheduler(int threads = 1)
        {
            if(threads < 0)
                throw new ArgumentException(nameof(threads));
            
            _threads = Enumerable.Range(0, threads).Select(x =>
            {
                var thread = new Thread(ThreadStart)
                {
                    IsBackground = true,
                    Name = $"Volume Thread #{x + 1}"
                };
                thread.SetApartmentState(ApartmentState.STA);
                return thread;
            }).ToList();

            _threads.ForEach(x => x.Start());
            _threadsIds = _threads.Select(x => x.ManagedThreadId).ToList();
        }
        
        #endregion
        
        #region Private Methods

        private void ThreadStart()
        {
            foreach (var task in _tasks.GetConsumingEnumerable())
                TryExecuteTask(task);
        }
        
        #endregion

        #region Implemented Methods
        
        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (!RequireInvoke)
                return false;

            return TryExecuteTask(task);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks.ToArray();    //copying tasks
        }
        
        #endregion
    }

}