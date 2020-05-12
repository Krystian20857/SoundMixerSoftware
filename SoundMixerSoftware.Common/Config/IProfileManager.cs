using System;
using System.Collections.Generic;

namespace SoundMixerAppv2.Common.Config
{
    public interface IProfileManager<T>
    {
        /// <summary>
        /// Stores loaded profiles.
        /// </summary>
        Dictionary<Guid, T> Profiles { get; set; }
        /// <summary>
        /// Load specified profile.
        /// </summary>
        /// <param name="uuid">uuid of profile</param>
        void Load(Guid uuid);
        /// <summary>
        /// Creates new profile.
        /// </summary>
        /// <param name="profile">profile struct</param>
        /// <returns>guid of created profile</returns>
        Guid Create(T profile);
        /// <summary>
        /// Saves profile.
        /// </summary>
        /// <returns>guid of created profile</returns>>
        bool Save(Guid uuid);
        /// <summary>
        /// Loads all profiles to <see cref="Profiles"/>.
        /// </summary>
        /// <returns>Returns true when profile has been created.</returns>
        void LoadAll();
        /// <summary>
        /// Save all profiles.
        /// </summary>
        void SaveAll();
        /// <summary>
        /// Get profiles.
        /// </summary>
        /// <returns>Returns available profiles</returns>
        IEnumerable<Guid> GetProfiles();
    }
}