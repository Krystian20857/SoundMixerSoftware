using System;

namespace SoundMixerSoftware.Common.Web
{
    public class FileDownloadEventArgs : EventArgs
    {
        public Uri Url { get; set; }
        public string FileName { get; set; }

        public FileDownloadEventArgs(Uri url, string fileName)
        {
            Url = url;
            FileName = fileName;
        }
    }

    public class DownloadProgressChanged : EventArgs
    {
        public long DownloadedBytes { get; set; }
        public long TotalBytes { get; set; }
        public double Percentage { get; set; }
        public double Speed { get; set; }
      
        public DownloadProgressChanged(long downloadedBytes, long totalBytes, double percentage, double speed)
        {
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
            Percentage = percentage;
            Speed = speed;
        }
      
    }
}