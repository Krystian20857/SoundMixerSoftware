using System;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Helpers.AudioSessions
{
    public enum SessionMode
    {
        SESSION,
        DEVICE,
        DEFAULT_COMMUNICATION,
        DEFAULT_MULTIMEDIA
    }

    public static class SessionModeExtension
    {
        /// <summary>
        /// Create unique id for SessionMode and DataFlow pair.
        /// </summary>
        /// <param name="sessionMode"></param>
        /// <param name="dataFlow"></param>
        /// <returns></returns>
        public static Guid CreateUUID(this SessionMode sessionMode, DataFlow dataFlow)
        {
            var a = (int)sessionMode ^ int.MaxValue;
            var b = (short)((((int) dataFlow & 0xFFFF0000) >> 16) ^ short.MaxValue);
            var c = (short) (((int) dataFlow & 0x0000FFFF) ^ short.MaxValue);
            var d = new byte[]{43, 56, 220, 45, 5, 255, 167, 123};
            return new Guid(a, b, c, d);
        }

        /// <summary>
        /// Create unique id for SessionMode and DataFlow pair and convert to string.
        /// </summary>
        /// <param name="sessionMode"></param>
        /// <param name="dataFlow"></param>
        /// <returns></returns>
        public static string CreateUUIDString(this SessionMode sessionMode, DataFlow dataFlow) => CreateUUID(sessionMode, dataFlow).ToString();
    }
}