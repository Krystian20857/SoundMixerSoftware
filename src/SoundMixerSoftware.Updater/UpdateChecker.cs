using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SoundMixerSoftware.Common.Web;
using SoundMixerSoftware.Updater.Github;

namespace SoundMixerSoftware.Updater
{
    public class UpdateChecker
    {
        #region Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields
        
        private readonly WebClient _client = new WebClient();
        private readonly Timer _checkTimer;
        
        #endregion
        
        #region Public Properties

        public Uri Url { get; }
        public Version CurrentVersion { get; }
        public NewVersionEventArgs LastRelease { get; protected set; }
        public bool UsePrerelease { get; set; }
        public TimeSpan CheckInterval { get; } = TimeSpan.FromDays(1);
        public bool NewVersion { get; protected set; }
        
        #endregion
        
        #region Events

        public event EventHandler<Exception> UpdateCheckError;
        public event EventHandler<NewVersionEventArgs> NewVersionAvailable;
        public event EventHandler<NewVersionEventArgs> NotNewVersionAvailable;
        
        #endregion
        
        #region Constructor

        public UpdateChecker(Version currentVersion, string url, bool usePrerelease = false)
        {
            Url = new Uri(url);
            CurrentVersion = currentVersion;
            UsePrerelease = usePrerelease;

            _client.DownloadStringCompleted += (sender, args) =>
            {
                if (args.Cancelled || args.Error != null)
                {
                    UpdateCheckError?.Invoke(this, args.Error);
                    Logger.Warn($"Error white getting new version: {args.Error}");
                    return;
                }

                var githubReleases = JsonConvert.DeserializeObject<List<GithubRelease>>(args.Result);

                var githubRelease = githubReleases.FirstOrDefault();
                if (githubRelease == default)
                {
                    Logger.Info("Cannot find new version.");
                    NewVersion = false;
                    NotNewVersionAvailable?.Invoke(this, new NewVersionEventArgs(default, new Release{ReleaseVersion = CurrentVersion}));
                    return;
                }
                
                var version = new Version(githubRelease.tag_name.Substring(1));
                if (githubRelease.prerelease && !UsePrerelease)
                {
                    Logger.Info($"Skipping pre-release: {version}");
                    var release1 = githubReleases.FirstOrDefault(x => !x.prerelease);
                    if (release1 == default)
                    {
                        Logger.Info("Cannot find non-prerelease version.");
                        NewVersion = false;
                        NotNewVersionAvailable?.Invoke(this, new NewVersionEventArgs(default, new Release{ReleaseVersion = CurrentVersion}));
                        return;
                    }

                    githubRelease = release1;
                }

                var eventArgs = new NewVersionEventArgs(githubRelease, ReleaseFromGithub(githubRelease));
                LastRelease = eventArgs;
                NewVersion = CurrentVersion < version;
                if (!NewVersion)
                {
                    Logger.Info("No update available.");
                    NewVersion = false;
                    NotNewVersionAvailable?.Invoke(this, eventArgs);
                    return;
                }
                
                NewVersionAvailable?.Invoke(this, eventArgs);
            };
            
            _checkTimer = new Timer(state =>
            {
                CheckForUpdate();
                _checkTimer?.Change(CheckInterval, CheckInterval);
            }, null, CheckInterval, CheckInterval);
            _checkTimer.Change(CheckInterval, CheckInterval);

        }

        public UpdateChecker(Version currentVersion, string url, TimeSpan checkInterval, bool usePrerelease = false) : this(currentVersion, url, usePrerelease)
        {
            CheckInterval = checkInterval;
        }
        
        #endregion
        
        #region Public Methods

        public Task CheckForUpdate()
        {
            if (_client.IsBusy)
                return Task.FromResult(false);
            try
            {
                _client.Headers.Add("User-Agent", WebUserAgent.Default);
                return _client.DownloadStringTaskAsync(Url);
            }
            catch (WebException exception)
            {
                UpdateCheckError?.Invoke(this, exception);
                return Task.FromResult(false);
            }
            
        }

        public Task<NewVersionEventArgs> GetCurrentVersion()
        {
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", WebUserAgent.Default);
            try
            {
                return Task<NewVersionEventArgs>.Factory.StartNew(() =>
                {
                    var releaseRaw = webClient.DownloadString($"{Url}/tags/v{CurrentVersion}");
                    var githubRelease = JsonConvert.DeserializeObject<GithubRelease>(releaseRaw);
                    if (githubRelease == default)
                    {
                        Logger.Warn("Cannot get current version from version server.");
                        return default;
                    }

                    return new NewVersionEventArgs(githubRelease, ReleaseFromGithub(githubRelease));

                });
            }
            catch (WebException exception)
            {
                UpdateCheckError?.Invoke(this, exception);
                return Task.FromResult((NewVersionEventArgs)null);
            }
        }

        public void CancelUpdateCheck()
        {
            _client.CancelAsync();
        }

        public static Release ReleaseFromGithub(GithubRelease githubRelease)
        {
            var version = new Version(githubRelease.tag_name.Substring(1));
            var asset = githubRelease.assets.FirstOrDefault(x => x.name.EndsWith(".exe"));
            if (asset == default)
            {
                Logger.Warn("Cannot find executable in release.");
                return default;
            }
            return new Release
            {
                ReleaseUrl = new Uri(githubRelease.html_url),
                Name = githubRelease.name,
                ReleaseVersion = version,
                DownloadLink = new Uri(asset.browser_download_url),
                Changes = githubRelease.body
            };
        }
        
        #endregion
    }
}