using System;
using System.Text.RegularExpressions;

namespace SoundMixerSoftware.Common.Utils.Audio
{
    public class AudioSessionUtil
    {
        #region Constant

        private const string UUIDRegex = @"([a-fA-F0-9]{8}[-][a-fA-F0-9]{4}[-][a-fA-F0-9]{4}[-][a-fA-F0-9]{4}[-][a-fA-F0-9]{12})";

        #endregion
        
        #region Static Methods
        
        public static Guid ParseDeviceId(string sessionId){
            var regex = new Regex(UUIDRegex);
            var uuids = regex.Matches(sessionId);
            if (uuids.Count == 0)
                return Guid.Empty;
            return Guid.TryParse(uuids[0].ToString(), out var uuid) ? uuid : Guid.Empty;
        }
        
        #endregion
    }
}