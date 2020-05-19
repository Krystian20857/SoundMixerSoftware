using System;
using System.Windows.Forms;

namespace SoundMixerSoftware.Win32.Utils
{
    public class NativeWindowWrapper : NativeWindow
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
    }
}