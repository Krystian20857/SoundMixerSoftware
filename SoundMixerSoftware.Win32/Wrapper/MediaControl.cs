using SoundMixerSoftware.Win32.Utils;
using SoundMixerSoftware.Win32.Win32;
using  VirtualKeyShort = SoundMixerSoftware.Win32.Win32.NativeEnums.VirtualKeyShort;

namespace SoundMixerSoftware.Win32.Wrapper
{
    public static class MediaControl
    {
        #region Private Static Fields
        
        private static readonly NativeStructs.INPUT PrevTractInput = KeyHelper.CreateKey(VirtualKeyShort.MEDIA_PREV_TRACK);
        private static readonly NativeStructs.INPUT NextTractInput = KeyHelper.CreateKey(VirtualKeyShort.MEDIA_NEXT_TRACK);
        private static readonly NativeStructs.INPUT PausePlayInput = KeyHelper.CreateKey(VirtualKeyShort.MEDIA_PLAY_PAUSE);
        private static readonly NativeStructs.INPUT StopInput = KeyHelper.CreateKey(VirtualKeyShort.MEDIA_STOP);

        #endregion
        
        #region Public Static Methods
        /// <summary>
        /// Go to previous track. 
        /// </summary>
        public static void PrevTrack()
        {
            KeyHelper.KeyClick(PrevTractInput);
        }

        /// <summary>
        /// Skip to next track.
        /// </summary>
        public static void NextTrack()
        {
            KeyHelper.KeyClick(NextTractInput);
        }

        /// <summary>
        /// Play/Pause media.
        /// </summary>
        public static void PauseResume()
        {
            KeyHelper.KeyClick(PausePlayInput);
        }
        
        /// <summary>
        /// Permanently stops all media.
        /// </summary>
        public static void Stop()
        {
            KeyHelper.KeyClick(StopInput);
        }
        
        #endregion
    }
}