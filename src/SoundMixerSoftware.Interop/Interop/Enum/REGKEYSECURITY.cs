// ReSharper disable InconsistentNaming
namespace SoundMixerSoftware.Interop.Enum
{
    /// <summary>
    /// Access rights for registry key objects.
    /// </summary>
    public enum REGKEYSECURITY
    {
        /// <summary>
            /// Combines the STANDARD_RIGHTS_REQUIRED, KEY_QUERY_VALUE, KEY_SET_VALUE, KEY_CREATE_SUB_KEY, KEY_ENUMERATE_SUB_KEYS, KEY_NOTIFY, and KEY_CREATE_LINK access rights.
            /// </summary>
            KEY_ALL_ACCESS = 0xF003F,

            /// <summary>
            /// Reserved for system use.
            /// </summary>
            KEY_CREATE_LINK = 0x0020,

            /// <summary>
            /// Required to create a subkey of a registry key.
            /// </summary>
            KEY_CREATE_SUB_KEY = 0x0004,

            /// <summary>
            /// Required to enumerate the subkeys of a registry key.
            /// </summary>
            KEY_ENUMERATE_SUB_KEYS = 0x0008,

            /// <summary>
            /// Equivalent to KEY_READ.
            /// </summary>
            KEY_EXECUTE = 0x20019,

            /// <summary>
            /// Required to request change notifications for a registry key or for subkeys of a registry key.
            /// </summary>
            KEY_NOTIFY = 0x0010,

            /// <summary>
            /// Required to query the values of a registry key.
            /// </summary>
            KEY_QUERY_VALUE = 0x0001,

            /// <summary>
            /// Combines the STANDARD_RIGHTS_READ, KEY_QUERY_VALUE, KEY_ENUMERATE_SUB_KEYS, and KEY_NOTIFY values.
            /// </summary>
            KEY_READ = 0x20019,

            /// <summary>
            /// Required to create, delete, or set a registry value.
            /// </summary>
            KEY_SET_VALUE = 0x0002,

            /// <summary>
            /// Indicates that an application on 64-bit Windows should operate on the 32-bit registry view. For more information, see Accessing an Alternate Registry View. This flag must be combined using the OR operator with the other flags in this table that either query or access registry values. Windows 2000:  This flag is not supported.
            /// </summary>
            KEY_WOW64_32KEY = 0x0200,

            /// <summary>
            /// Indicates that an application on 64-bit Windows should operate on the 64-bit registry view. For more information, see Accessing an Alternate Registry View. This flag must be combined using the OR operator with the other flags in this table that either query or access registry values. Windows 2000:  This flag is not supported.
            /// </summary>
            KEY_WOW64_64KEY = 0x0100,

            /// <summary>
            /// Combines the STANDARD_RIGHTS_WRITE, KEY_SET_VALUE, and KEY_CREATE_SUB_KEY access rights.
            /// </summary>
            KEY_WRITE = 0x20006
    }
}