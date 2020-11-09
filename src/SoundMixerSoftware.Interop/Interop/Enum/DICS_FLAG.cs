// ReSharper disable InconsistentNaming
namespace SoundMixerSoftware.Interop.Enum
{
    public enum DICS_FLAG
    {
        /// <summary>
        /// make change in all hardware profiles
        /// </summary>
        DICS_FLAG_GLOBAL = 0x00000001,

        /// <summary>
        /// make change in specified profile only
        /// </summary>
        DICS_FLAG_CONFIGSPECIFIC = 0x00000002,

        /// <summary>
        /// 1 or more hardware profile-specific
        /// </summary>
        DICS_FLAG_CONFIGGENERAL = 0x00000004,
    }
}