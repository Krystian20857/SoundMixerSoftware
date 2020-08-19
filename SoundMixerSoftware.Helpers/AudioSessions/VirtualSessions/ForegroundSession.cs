using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Helpers.Annotations;
using SoundMixerSoftware.Win32.Threading;

namespace SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions
{
    public class ForegroundSession : IVirtualSession, INotifyPropertyChanged
    {
        #region Private Fields

        private Dispatcher _dispatcher = Application.Current.Dispatcher;
        private WindowWatcher _windowWatcher;

        #endregion
        
        #region Implemented Properties

        public string Key { get; } = "foreground-session";
        public string DisplayName { get; set; }
        public string ID { get; } = "B0BA17A8-0CC4-458E-90F4-385794DE41FC";
        public int Index { get; }
        public Guid UUID { get; }
        public ImageSource Image { get; set; }
        public SessionState State { get; set; }
        public float Volume { get; set; }
        public bool IsMute { get; set; }
        
        #endregion
        
        #region Public Properties

        public AudioSessionControl Session { get; set; }
        public IntPtr WindowHandle { get; set; }

        #endregion
        
        #region Implemented Events
        
        public event EventHandler<VolumeChangedArgs> VolumeChange;
        public event EventHandler<MuteChangedArgs> MuteChanged;
        
        #endregion
        
        #region Constructor

        public ForegroundSession(int sliderIndex, Guid uuid)
        {
            UUID = uuid;
            Index = sliderIndex;

            _windowWatcher = _dispatcher.Invoke(() => new WindowWatcher());
            _windowWatcher.ForegroundWindowChanged += (sender, args) =>
            {
                _dispatcher.Invoke(() =>
                {
                    Debug.WriteLine($"Foreground process id: {args.ProcessId}");
                    foreach (var sessionEnum in SessionHandler.SessionEnumerators.Values)
                    {
                        var sessions = sessionEnum.AudioSessions;
                        for (var n = 0; n < sessions.Count; n++)
                        {
                            var session = sessions[n];
                            using (var process = Process.GetProcessById((int) session.GetProcessID))
                            {
                                Debug.WriteLine($"Audio session: Id: {process.Id} Name: {process.GetPreciseName()}");
                            }
                        }
                    }
                });
            };
        }
        
        #endregion

        #region Implemented Methods

        public Dictionary<object, object> Save()
        {
            return new Dictionary<object, object>();
        }

        #endregion
        
        #region Private Methods

        #endregion
        
        #region Private Events
        
        

        #endregion
        
        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }

    public class ForegroundSessionCreator : IVirtualSessionCreator
    {
        public IVirtualSession CreateSession(int index, Dictionary<object, object> container, Guid uuid)
        {
            return new ForegroundSession(index, uuid);
        }
    }
}