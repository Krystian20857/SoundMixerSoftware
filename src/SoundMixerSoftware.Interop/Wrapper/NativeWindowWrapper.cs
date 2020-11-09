using System;
using System.Windows.Forms;

namespace SoundMixerSoftware.Interop.Wrapper
{
    public sealed class NativeWindowWrapper : NativeWindow, IDisposable
    {
        #region Events

        public event EventHandler<Message> MessageReceived;
        
        #endregion
        
        #region Constructor

        public NativeWindowWrapper()
        {
            CreateHandle(new CreateParams());
        }

        #endregion
        
        #region Window Message Proc

        protected override void WndProc(ref Message m)
        {
            MessageReceived?.Invoke(this, m);
            base.WndProc(ref m);
        }

        #endregion

        #region Dispose

        ~NativeWindowWrapper()
        {
            DestroyHandle();
        }
        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        #endregion
    }
}