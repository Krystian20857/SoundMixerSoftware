using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.Profile;

namespace SoundMixerSoftware.Helpers.Utils
{
    public static class SessionUtil
    {
        #region Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        /// <summary>
        /// Add virtual session to slider and create profile entry.
        /// </summary>
        /// <param name="sliderIndex"></param>
        /// <param name="virtualSession"></param>
        /// <returns></returns>
        public static bool AddSession(int sliderIndex, IVirtualSession virtualSession)
        {
            if (virtualSession == null)
                return false;
            var profile = ProfileHandler.SelectedProfile;

            if(profile.Sliders == null)
                profile.Sliders = new List<SliderStruct>();
            if(profile.Sliders.Count <= sliderIndex)
                for(var n = profile.Sliders.Count; n < sliderIndex + 1; n++)
                    profile.Sliders.Add(new SliderStruct());

            try
            {
                var slider = SessionHandler.Sessions[sliderIndex];
                if (slider.Any(x => x.ID == virtualSession.ID))
                    return false;
                
                var session = SessionHandler.AddSession(sliderIndex, virtualSession);
                profile.Sliders[sliderIndex].Sessions.Add(session);
                ProfileHandler.SaveSelectedProfile();
            }
            catch (Exception exception)
            {
                ExceptionHandler.HandleException(Logger, exception.Message, exception);
                return false;
            }

            return true;
        }
        
    }
}