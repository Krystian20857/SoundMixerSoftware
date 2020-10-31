using System;
// ReSharper disable InconsistentNaming

namespace SoundMixerSoftware.Win32.Interop.Enum
{
    /// <summary>
    /// Flags controlling what is included in the device information set built by SetupDiGetClassDevs
    /// </summary>
    [Flags]
    public enum DIGCF
    {
        DIGCF_DEFAULT = 0x00000001, // only valid with DIGCF_DEVICEINTERFACE
        DIGCF_PRESENT = 0x00000002,
        DIGCF_ALLCLASSES = 0x00000004,
        DIGCF_PROFILE = 0x00000008,
        DIGCF_DEVICEINTERFACE = 0x00000010,
    }
}