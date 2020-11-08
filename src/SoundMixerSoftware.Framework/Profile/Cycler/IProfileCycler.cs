using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Framework.Profile.Cycler
{
    public interface IProfileCycler
    {
        Guid GetNextProfile(IEnumerable<Guid> profiles, Guid selectedProfile);
        Guid NextProfile();
    }
}