using System;
using SoundMixerSoftware.Win32.Interop.Struct;

namespace SoundMixerSoftware.Win32.Interop.Constant
{
    public static class PropertyKeys
    {
        public static PROPERTYKEY PKEY_ItemNameDisplay = new PROPERTYKEY
        {
            fmtid = Guid.Parse("{B725F130-47EF-101A-A5F1-02608C9EEBAC}"),
            pid = new UIntPtr(10)
        };
    }
}