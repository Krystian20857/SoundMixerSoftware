using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NLog;

namespace SoundMixerSoftware.Common.Web
{
    public class FileDownloader : IDisposable
    {
        #region Logger

        public static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields
        
        private WebClient _webClient = new WebClient();
        private DateTime lastChecked;
        private long lastBytes;
        
        private int avgCount;
        private double cachedSpeed;
        private double lastSpeed;

        #endregion
        
        #region Public Properties

        public string FileName { get; set; }
        public Uri Url { get; set; }
        public int SampleCount { get; set; } = 8;
        public bool IsDownloading => _webClient.IsBusy;

        #endregion
        
        #region Events

        public event EventHandler<Exception> DownloadError;
        public event EventHandler<FileDownloadEventArgs> FileDownloaded;
        public event EventHandler<DownloadProgressChanged> ProgressChanged;
        
        #endregion
        
        #region Constructor

        public FileDownloader()
        {
            _webClient.DownloadFileCompleted += (sender, args) =>
            {
                if (args.Cancelled || args.Error != null)
                {
                    DownloadError?.Invoke(this, args.Error);
                    Logger.Warn($"Errow while downloading file from: {Url}");
                    return;
                }
                if (!File.Exists(FileName))
                {
                    Logger.Warn($"Errow while downloading file from: {Url} to {FileName}");
                    return;
                }
                Logger.Info($"Succefully downloaded file from {Url} to {FileName}");
                FileDownloaded?.Invoke(this, new FileDownloadEventArgs(Url, FileName));
            };

            _webClient.DownloadProgressChanged += (sender, args) =>
            {
                var downloadedBytes = args.BytesReceived;
                var totalBytes = args.TotalBytesToReceive;
                
                var timeDiff = DateTime.Now - lastChecked;
                if(timeDiff.TotalMilliseconds <= 50 && totalBytes - downloadedBytes > 0)
                    return;
                var progress = (double)downloadedBytes / totalBytes * 100;
                var bytesDiff = downloadedBytes - lastBytes;
                var speed = bytesDiff / timeDiff.TotalSeconds;

                if (avgCount == SampleCount)
                {
                    speed = cachedSpeed / SampleCount;
                    lastSpeed = speed;
                    avgCount = 0;
                    cachedSpeed = 0;
                }
                else
                {
                    if(speed > 0 && !double.IsInfinity(speed))
                        cachedSpeed += speed;
                    if (Math.Abs(lastSpeed) < 0.005)
                        lastSpeed = speed;
                    speed = lastSpeed;
                    avgCount++;
                }

                lastBytes = args.BytesReceived;
                lastChecked = DateTime.Now;

                ProgressChanged?.Invoke(this, new DownloadProgressChanged(downloadedBytes, totalBytes, progress, speed));
            };
        }
        
        #endregion
        
        #region Public Methods

        public void DownloadFileAsync()
        {
            try
            {
                lastChecked = DateTime.Now;
                _webClient.Headers.Add("User-Agent", WebUserAgent.Default);
                _webClient.DownloadFileAsync(Url, FileName);
            }
            catch (WebException exception)
            {
                DownloadError?.Invoke(this, exception);
            }
        }
        
        public void DownloadFileAsync(Uri url, string fileName)
        {
            Url = url;
            FileName = fileName;
            DownloadFile();
        }

        public void DownloadFileAsync(string url, string fileName)
        {
           DownloadFileAsync(new Uri(url), fileName);
        }

        public Task DownloadFile()
        {
            try
            {
                _webClient.Headers.Add("User-Agent", WebUserAgent.Default);
                return _webClient.DownloadFileTaskAsync(Url, FileName);
            }
            catch (WebException exception)
            {
                DownloadError?.Invoke(this, exception);
                return Task.FromResult(false);
            }
        }
        
        public Task DownloadFile(Uri url, string fileName)
        {
            try
            {
                Url = url;
                FileName = fileName;
                return DownloadFile();
            }
            catch (WebException exception)
            {
                DownloadError?.Invoke(this, exception);
                return Task.FromResult(false);
            }
        }
        
        public Task DownloadFile(string url, string fileName)
        {
            return DownloadFile(new Uri(url), fileName);
        }

        public void CancelDownload()
        {
            _webClient.CancelAsync();
        }
        
        #endregion

        #region Dispose
        
        public void Dispose()
        {
            _webClient.Dispose();
        }
        
        #endregion
    }
}