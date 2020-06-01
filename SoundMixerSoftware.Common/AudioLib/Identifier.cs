namespace SoundMixerSoftware.Common.AudioLib
{
    /// <summary>
    /// This class contains methods useful with audio ids.
    /// </summary>
    public static class Identifier
    {
        public static string GetDeviceId(string sessionId)
        {
            var segments = sessionId.Split('|');
            if (segments.Length == 0)
                return string.Empty;
            return segments[0];
        }
        
        /// <summary>
        /// Compare ids for sessions, do not include device id.
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        public static bool Compare(string id1, string id2)
        {
            return string.Equals(id1.Split('|')[1], id2.Split('|')[1]);
        }
    }
}