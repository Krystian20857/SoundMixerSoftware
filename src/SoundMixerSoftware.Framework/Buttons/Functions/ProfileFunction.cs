using System;
using System.Collections.Generic;
using System.Windows.Media;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils;
using SoundMixerSoftware.Common.Utils.EnumUtils;
using SoundMixerSoftware.Framework.Overlay;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.Profile.Cycler;
using SoundMixerSoftware.Resource.Image;

namespace SoundMixerSoftware.Framework.Buttons.Functions
{
    public class ProfileFunction : IButtonFunction
    {
        #region Const

        public const string KEY = "profile_func";

        public const string TASK_KEY = "Task";
        public const string PROFILE_UUID_KEY = "ProfileUUID";
            
        #endregion
        
        #region Implemented Properties

        public string Name
        {
            get
            {
                switch (ProfileTask)
                {
                    case ProfileFuncTask.CYCLE:
                        return "Profile Cycle";
                    case ProfileFuncTask.SET_PROFILE:
                        var profileExists = ProfileHandler.ProfileManager.Profiles.ContainsKey(ProfileUUID);
                        var profileName = profileExists ? ProfileHandler.ProfileManager.Profiles[ProfileUUID].Name : "(Profile Not Exists)";
                        return $"Set Profile: {profileName}";
                    default:
                        return "Profile Function(Error)";
                }
            }
            set{}
        }
        public string Key { get; } = KEY;
        public int Index { get; set; }
        public Guid UUID { get; set; }
        public ImageSource Image { get; set; } = Images.Profile;
        
        #endregion
        
        #region Public Properties

        public ProfileFuncTask ProfileTask { get; }
        public Guid ProfileUUID { get; set; } = Guid.Empty;

        #endregion
        
        #region Constructor

        public ProfileFunction(int index, Guid uuid)
        {
            Index = index;
            ProfileTask = ProfileFuncTask.CYCLE;
            UUID = uuid;
        }

        public ProfileFunction(int index, Guid profileUUID, Guid uuid)
        {
            Index = index;
            ProfileTask = ProfileFuncTask.SET_PROFILE;
            ProfileUUID = profileUUID;
            UUID = uuid;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(TASK_KEY, ProfileTask);
            result.Add(PROFILE_UUID_KEY, ProfileUUID);
            return result;
        }

        public void ButtonKeyDown(int index)
        {
            switch (ProfileTask)
            {
                case ProfileFuncTask.CYCLE:
                    var profile = TaskUtil.InvokeDispatcher(() => DefaultProfileCycler.Instance.NextProfile());
                    OverlayHandler.ShowProfile(profile);
                    break;
                case ProfileFuncTask.SET_PROFILE:
                    var profiles = ProfileHandler.ProfileManager.Profiles;
                    if (profiles.ContainsKey(ProfileUUID))
                    {
                        TaskUtil.BeginInvokeDispatcher(() => ProfileHandler.OnProfileChanged(ProfileUUID));
                        OverlayHandler.ShowProfile(ProfileUUID);
                    }
                    break;
            }
        }

        public void ButtonKeyUp(int index)
        {
            
        }
        
        #endregion
        
        #region Private Methods

        #endregion
    }

    public class ProfileFunctionCreator : IButtonCreator
    {
        public IButtonFunction CreateButton(int index, Dictionary<object, object> container, Guid uuid)
        {
            if (!container.TryGetValue(ProfileFunction.TASK_KEY, out var taskObj))
                throw new ArgumentException("Cannot find function: " + ProfileFunction.TASK_KEY);
            var task = EnumUtil.Parse<ProfileFuncTask>(taskObj.ToString());
            switch (task)
            {
                case ProfileFuncTask.CYCLE:
                    return new ProfileFunction(index, uuid);
                case ProfileFuncTask.SET_PROFILE:
                    var profileUUIDObj = container.ContainsKey(ProfileFunction.PROFILE_UUID_KEY) ? container[ProfileFunction.PROFILE_UUID_KEY] : Guid.Empty;
                    var profileUUID = Guid.Parse(profileUUIDObj?.ToString() ?? string.Empty);
                    return new ProfileFunction(index, profileUUID, uuid);
                default:
                    return null;
            }
        }
    }

    public enum ProfileFuncTask
    {
        [ValueName("Cycle profiles")]CYCLE,
        [ValueName("Set profile")]SET_PROFILE
    }
}