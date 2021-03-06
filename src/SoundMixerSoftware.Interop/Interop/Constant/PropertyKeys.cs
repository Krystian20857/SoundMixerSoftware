﻿using System;
using SoundMixerSoftware.Interop.Struct;

// ReSharper disable InconsistentNaming

namespace SoundMixerSoftware.Interop.Constant
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