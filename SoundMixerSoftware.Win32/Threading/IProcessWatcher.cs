using System;

namespace SoundMixerSoftware.Common.Threading
{
    public interface IProcessWatcher : IDisposable
    {
        /// <summary>
        /// Attach exit action to process.
        /// </summary>
        /// <param name="processId">Id of process.</param>
        /// <param name="exitAction">Exit action delegate.</param>
        void AttachProcessWait(int processId, Action<int> exitAction);
        /// <summary>
        /// Detach exit action from process.
        /// </summary>
        /// <param name="processId">Id of process.</param>
        bool DetachProcessWait(int processId);
    }
}