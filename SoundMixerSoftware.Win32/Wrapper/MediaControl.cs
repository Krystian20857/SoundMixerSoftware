using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;

namespace SoundMixerSoftware.Win32.Wrapper
{
    public static class MediaControl
    {
        #region Private Static Fields
        
        private static readonly KeyboardSimulator _keyboardSimulator = new KeyboardSimulator();
        
        #endregion
        
        #region Public Static Methods
        /// <summary>
        /// Go to previous track. 
        /// </summary>
        public static void PrevTrack()
        {
            _keyboardSimulator.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);
        }

        /// <summary>
        /// Skip to next track.
        /// </summary>
        public static void NextTrack()
        {
            _keyboardSimulator.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
        }

        /// <summary>
        /// Play/Pause media.
        /// </summary>
        public static void PauseResume()
        {
            _keyboardSimulator.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
        }
        
        /// <summary>
        /// Permanently stops all media.
        /// </summary>
        public static void Stop()
        {
            _keyboardSimulator.KeyPress(VirtualKeyCode.MEDIA_STOP);
        }
        
        #endregion
    }
}