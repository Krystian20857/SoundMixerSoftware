using System;
using SoundMixerSoftware.Updater.Github;

namespace SoundMixerSoftware.Updater
{
    public class Release
    {
        public Version ReleaseVersion { get; set; }
        public string Name { get; set; }
        public Uri DownloadLink { get; set; }
        public Uri ReleaseUrl { get; set; }
        public string Changes { get; set; }

    }
}