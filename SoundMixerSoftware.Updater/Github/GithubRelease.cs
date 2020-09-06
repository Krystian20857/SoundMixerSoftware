using System.Collections.Generic;

namespace SoundMixerSoftware.Updater.Github
{
    public class GithubRelease
    {
        public string html_url { get; set; }
        public string body { get; set; }
        public string tag_name { get; set; }
        public string name { get; set; }
        public bool prerelease { get; set; }
        public List<Asset> assets { get; set; }

        public class Asset
        {
            public string name { get; set; }
            public string browser_download_url { get; set; }
        }

        public override string ToString()
        {
            return $"Name: {name} Tag: {tag_name} Url: {html_url}";
        }
    }
}