using System;

namespace SoundMixerSoftware.Interop.Wrapper
{
    public static class SystemVersion
    {
        /// <summary>
        /// Returns true when system version is win 8.1 or higher(win 10).
        /// </summary>
        /// <returns></returns>
        public static bool IsWin8OrHigher()
        {
            var majorVersion = Environment.OSVersion.Version.Major;
            var minorVersion = Environment.OSVersion.Version.Minor;
            var platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Win32NT)
                return (majorVersion == 6 && minorVersion == 3) || (majorVersion == 10 && minorVersion == 0);
            return false;
        }
    }
}