using System;
using System.IO;
using System.Threading.Tasks;
using NLog;
using SoundMixerSoftware.Common.Web;
using SoundMixerSoftware.Updater.Installer;

namespace SoundMixerSoftware.Updater
{
    public class Updater
    {
        #region Logger
        
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields

        private readonly UpdateChecker _updateChecker;
        private readonly InstallerStarter _starter;

        private readonly FileDownloader _downloader = new FileDownloader();

        private string _fileName;

        #endregion

        #region Public Properties

        public UpdateMode Mode { get; set; } = UpdateMode.NORMAL;
        public bool HasNewVersion => _updateChecker.NewVersion;
        public NewVersionEventArgs LastRelease => _updateChecker.LastRelease;
        public string DownloadCache { get; set; }
        public bool IsDownloading => _downloader.IsDownloading;

        #endregion
        
        #region Events

        public event EventHandler<NewVersionEventArgs> NewVersionAvailable
        {
            add => _updateChecker.NewVersionAvailable += value;
            remove => _updateChecker.NewVersionAvailable -= value;
        }
        public event EventHandler<Exception> UpdateCheckError
        {
            add => _updateChecker.UpdateCheckError += value;
            remove => _updateChecker.UpdateCheckError -= value;
        }
        public event EventHandler<NewVersionEventArgs> NotNewVersionAvailable
        {
            add => _updateChecker.NotNewVersionAvailable += value;
            remove => _updateChecker.NotNewVersionAvailable -= value;
        }
        public event EventHandler<Exception> DownloadError
        {
            add => _downloader.DownloadError += value;
            remove => _downloader.DownloadError -= value;
        }
        public event EventHandler<FileDownloadEventArgs> FileDownloaded
        {
            add => _downloader.FileDownloaded += value;
            remove => _downloader.FileDownloaded -= value;
        }
        public event EventHandler<DownloadProgressChanged> ProgressChanged
        {
            add => _downloader.ProgressChanged += value;
            remove => _downloader.ProgressChanged -= value;
        }
        
        #endregion

        #region Constructor


        public Updater(Version currentVersion, string url, string cacheDir, Action<InstallerStarter> appExit)
        {
            DownloadCache = cacheDir;
            _updateChecker = new UpdateChecker(currentVersion, url);
            _starter = new InstallerStarter { AppExit = appExit };
        }

        #endregion

        #region Public Methods

        public Task CheckForUpdate()
        {
            return _updateChecker.CheckForUpdate();
        }

        public void DownloadUpdate()
        {
            foreach(var file in Directory.GetFiles(DownloadCache))
                File.Delete(file);
            _fileName = GetInstallerName();
            var lastRelease = _updateChecker.LastRelease.Release.DownloadLink;
            _downloader.CancelDownload();
            _downloader.DownloadFileAsync(lastRelease, _fileName);
        }

        public void RunInstaller()
        {
            _starter.FileName = _fileName;
            _starter.RunInstaller(Mode);
        }

        public void CancelInstall()
        {
            _updateChecker.CancelUpdateCheck();
            _downloader.CancelDownload();
        }

        #endregion

        #region Private Fields

        private string GetInstallerName()
        {
            var guid = Guid.NewGuid();
            while (File.Exists(Path.Combine(DownloadCache, guid + ".exe")))
            {
                guid = Guid.NewGuid();
            }

            return Path.Combine(DownloadCache, guid + ".exe");
        }

        #endregion
    }
}