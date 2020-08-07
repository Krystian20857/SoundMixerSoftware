using System;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions
{
    public enum DefaultDeviceMode
    {
        DEFAULT_MULTIMEDIA,
        DEFAULT_COMMUNICATION
    }
    
    public static class DefaultDeviceModeExtension{

        public static Guid CreateUUID(this DefaultDeviceMode mode, DataFlow dataFlow)
        {
            var a = (int)mode ^ int.MaxValue;
            var b = (short)((((int) dataFlow & 0xFFFF0000) >> 16) ^ short.MaxValue);
            var c = (short) (((int) dataFlow & 0x0000FFFF) ^ short.MaxValue);
            var d = new byte[]{43, 56, 220, 45, 5, 255, 167, 123};
            return new Guid(a, b, c, d);
        }

        public static string CreateStringUUID(this DefaultDeviceMode mode, DataFlow dataFlow) => CreateUUID(mode, dataFlow).ToString();

    }
}