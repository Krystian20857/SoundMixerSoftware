using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundMixerSoftware.Framework.Profile.Cycler
{
    public class DefaultProfileCycler : IProfileCycler
    {
        #region Static Properties

        public static IProfileCycler Instance { get; } = new DefaultProfileCycler();

        #endregion
        
        #region Private Fields

        private IEnumerable<Guid> _profiles => ProfileHandler.ProfileManager.Profiles.Keys;
        private Guid _selectedProfile => ProfileHandler.SelectedGuid;
        
        #endregion

        #region Implemented Methods
        
        public Guid GetNextProfile(IEnumerable<Guid> profiles, Guid selectedProfile)
        {
            var next = profiles.SkipWhile(x => x != selectedProfile).Skip(1).FirstOrDefault();
            if (next == default)
                next = profiles.ElementAt(0);
            return next;
        }

        public Guid NextProfile()
        {
            var next = GetNextProfile(_profiles, _selectedProfile);
            ProfileHandler.OnProfileChanged(next);
            return next;
        }
        
        #endregion
    }
}