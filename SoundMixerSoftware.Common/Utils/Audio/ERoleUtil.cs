﻿using AudioSwitcher.AudioApi;

namespace SoundMixerSoftware.Common.Utils.Audio
{
    public static class ERoleUtil
    {
        
        public static void GetFromRole(Role role, out bool isDefault, out bool isDefaultCommunications)
        {
            isDefault = false;
            isDefaultCommunications = false;
            
            if ((role & Role.Communications) == Role.Communications)
                isDefaultCommunications = true;
            else if ((role & Role.Multimedia) == Role.Multimedia)
                isDefault = true;
        }

        public static Role GetRole(bool isDefault, bool isDefaultCommunications)
        {
            var result = Role.Console;
            if (isDefault) result |= Role.Multimedia;
            if (isDefaultCommunications) result |= Role.Communications;
            return result;
        }
        
    }
}