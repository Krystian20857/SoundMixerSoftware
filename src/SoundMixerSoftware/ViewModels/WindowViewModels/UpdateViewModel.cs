using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;

namespace SoundMixerSoftware.ViewModels
{
    public class UpdateViewModel : Screen
    {
        #region Private Fields

        private double _downloadPercent;
        private double _downloadSpeed;
        private SolidColorBrush _statusColor;
        private string _status;
        
        #endregion
        
        #region Public Properties

        public static UpdateViewModel Instance => IoC.Get<UpdateViewModel>();

        public double DownloadPercent
        {
            get => _downloadPercent;
            set
            {
                _downloadPercent = value;
                NotifyOfPropertyChange(nameof(DownloadPercent));
            }
        }

        public double DownloadSpeed
        {
            get => _downloadSpeed;
            set
            {
                _downloadSpeed = value;
                NotifyOfPropertyChange(nameof(DownloadSpeed));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyOfPropertyChange(nameof(Status));
            }
        }

        public SolidColorBrush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                NotifyOfPropertyChange(nameof(StatusColor));
            }
        }
        
        public Updater.Updater Updater => Bootstrapper.Instance.Updater;

        #endregion


        #region Constructor

        public UpdateViewModel()
        {
            Updater.ProgressChanged += (sender, args) => Execute.OnUIThread(() =>
            {
                DownloadPercent = Math.Round(args.Percentage, 1);
                DownloadSpeed = args.Speed;
            });
            Updater.DownloadError += (sender, exception) =>
            {
                Execute.OnUIThread(() =>
                {
                    if (exception is WebException webException)
                    {
                        if (webException.Status == WebExceptionStatus.RequestCanceled)
                        {
                            Status = "Download Canceled";
                            StatusColor = new SolidColorBrush(Colors.IndianRed);
                            return;
                        }
                    }
                    Status = "Download Error";
                    StatusColor = new SolidColorBrush(Colors.IndianRed);
                });
            };
        }

        #endregion

        #region Events

        public void CancelClick()
        {
            Updater.CancelInstall();
        }
        
        public void InstallClick()
        {
            Updater.RunInstaller();
            TryCloseAsync();
        }

        #endregion
        
        #region Overrides

        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (!Updater.IsDownloading)
                return Task.FromResult(true);
            var result = MessageBox.Show("Do you want to cancel update download.", "Update...", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (result == MessageBoxResult.Yes)
            {
                Updater.CancelInstall();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        #endregion
        
        #region Public Methods

        public void StartUpdate()
        {
            Updater.DownloadUpdate();
        }
        
        #endregion
    }
}