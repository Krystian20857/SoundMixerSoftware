using System;
using Caliburn.Micro;

namespace SoundMixerSoftware.Utils
{
    public class ExtendedContainer : SimpleContainer
    {
        #region Events

        public event EventHandler<UnhandledExceptionEventArgs> OnException;
        
        #endregion
        
        #region Overriden Methods

        protected override object ActivateInstance(Type type, object[] args)
        {
            try
            {
                return base.ActivateInstance(type, args);
            }
            catch (Exception exception)
            {
                OnException?.Invoke(this, new UnhandledExceptionEventArgs(exception, false));
                throw;
            }
        }

        #endregion
    }
}