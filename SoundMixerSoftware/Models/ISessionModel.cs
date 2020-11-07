using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.Observables;
using SoundMixerSoftware.Annotations;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils.Audio;
using SoundMixerSoftware.Framework.Audio;
using SoundMixerSoftware.Framework.Audio.VirtualSessions;
using SoundMixerSoftware.Framework.Utils;
using SoundMixerSoftware.Resource.Image;

namespace SoundMixerSoftware.Models
{
    public interface ISessionModel
    {
        /// <summary>
        /// Id of session model for differentiation.
        /// </summary>
        string ID { get; set; }
        /// <summary>
        /// Unique identifier of model.
        /// </summary>
        Guid Guid { get; set; }
        /// <summary>
        /// Name of session model.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Image of session model.
        /// </summary>
        ImageSource Image { get; set; }
        /// <summary>
        /// Creates instance of virtual session from session model.
        /// </summary>
        /// <param name="sliderIndex">Index of slider adding.</param>
        /// <returns></returns>
        IVirtualSession CreateSession(int sliderIndex);
    }

    public class DefaultDeviceModel : ISessionModel, INotifyPropertyChanged
    {
        #region Private Fields

        private Dispatcher _dispatcher = Application.Current.Dispatcher;
        private IAudioController _controller = SessionHandler.AudioController;
        private IDevice _device;
        private bool isDefault;
        private bool isDefaultCommuncations;
        
        #endregion
        
        #region Implemented Properties

        public string ID { get; set; }
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public ImageSource Image { get; set; }
        public DeviceType Type { get; set; }
        public Role Role { get; set; }

        #endregion
        
        #region Constructor
        
        public DefaultDeviceModel(DeviceType type, Role role)
        {
            Type = type;
            Role = role;
            UpdateView(_controller.GetDefaultDevice(type, role));
            ERoleUtil.GetFromRole(role, out isDefault, out isDefaultCommuncations);
            
            _controller.AudioDeviceChanged.Subscribe(x =>
            {
               if(x.ChangedType != DeviceChangedType.DefaultChanged)
                   return;
               var args = x as DefaultDeviceChangedArgs;
               if (args.IsDefault && isDefault || args.IsDefaultCommunications && isDefaultCommuncations)
               {
                   UpdateView(x.Device);
               }
            });
        }

        #endregion
        
        #region Private Methods

        private void UpdateView(IDevice device)
        {
            if (device == null)
                return;
            if (device.DeviceType != Type)
                return;
            _device = device;
            _dispatcher.Invoke(() =>
            {
                try
                {
                    switch (Type)
                    {
                        case DeviceType.Playback:
                            switch (Role)
                            {
                                case Role.Multimedia:
                                    Name = $"Default Playback Device({device.FullName})";
                                    Image = Images.FailedEmbed;
                                    break;
                                case Role.Communications:
                                    Name = $"Default Communication Playback Device({device.FullName})";
                                    Image = Images.FailedEmbed;
                                    break;
                            }

                            break;
                        case DeviceType.Capture:
                            switch (Role)
                            {
                                case Role.Multimedia:
                                    Name = $"Default Capture Device({device.FullName})";
                                    Image = Images.FailedEmbed;
                                    break;
                                case Role.Communications:
                                    Name = $"Default Communication Device({device.FullName})";
                                    Image = Images.FailedEmbed;
                                    break;
                            }

                            break;
                    }
                }
                finally
                {
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(Image));
                }
            });
        }

        #endregion

        #region Implemented Methods
        
        public IVirtualSession CreateSession(int sliderIndex)
        {
            if (_device == default || (!_device.IsDefaultDevice && !_device.IsDefaultCommunicationsDevice))
                return null;
            return new DefaultDeviceSession(sliderIndex, Role, Type, Guid.NewGuid());
        }
        
        #endregion
        
        #region Property Change

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }

    public class AudioDeviceModel : ISessionModel
    {
        #region Implemented Properties
        
        public string ID { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public DeviceType Type { get; set; }

        #endregion
        
        #region Implemented Methods
        
        public IVirtualSession CreateSession(int sliderIndex)
        {
            return new DeviceSession(sliderIndex, Guid, Name, Guid.NewGuid());
        }
        
        #endregion
    }
    
    public class AudioSessionModel : ISessionModel
    {
        #region Implemented Properties
        
        public string ID { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        
        #endregion

        #region Implemented Methods
        
        public virtual IVirtualSession CreateSession(int sliderIndex)
        {
            return new VirtualSession(sliderIndex, ID, Name, Guid.NewGuid());
        }
        
        #endregion
    }

    public class ProcessSessionModel : ISessionModel
    {
        #region Implemented Properties
        
        public string ID { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        
        #endregion
        
        #region Public Properties

        public string ExecutablePath { get; set; }
        public string RawName { get; set; }

        #endregion

        #region Implemented Methods
        
        public virtual IVirtualSession CreateSession(int sliderIndex)
        {
            return new ProcessSession(sliderIndex, ExecutablePath, RawName, Guid.NewGuid());
        }
        
        #endregion
    }
    
    public class ForegroundSessionModel : ISessionModel
    {
        public string ID { get; set; } = "B0BA17A8-0CC4-458E-90F4-385794DE41FC";
        public Guid Guid { get; set; } = Guid.Parse("B0BA17A8-0CC4-458E-90F4-385794DE41FC");
        public string Name { get; set; } = "Foreground Session";
        public ImageSource Image { get; set; } = Images.Cog;
        public IVirtualSession CreateSession(int sliderIndex)
        {
            return new ForegroundSession(sliderIndex, Guid.NewGuid());
        }
    }
    
}