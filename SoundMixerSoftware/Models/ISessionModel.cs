using System;
using System.Windows.Media;
using NAudio.CoreAudioApi;
using SoundMixerSoftware.Helpers.AudioSessions;
using SoundMixerSoftware.Helpers.AudioSessions.VirtualSessions;

namespace SoundMixerSoftware.Models
{
    public interface ISessionModel
    {
        /// <summary>
        /// Id of session model for differentiation.
        /// </summary>
        string ID { get; set; }
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
    
    public class DefaultDeviceModel : ISessionModel
    {
        #region Implemented Properties
        
        public string ID { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public DefaultDeviceMode Mode { get; set; }
        public DataFlow DataFlow { get; set; }
        
        #endregion
        
        #region Implemented Methods
        
        public IVirtualSession CreateSession(int sliderIndex)
        {
            return new DefaultDeviceSession(sliderIndex, Mode, DataFlow, Guid.NewGuid());
        }
        
        #endregion
    }

    public class AudioDeviceModel : ISessionModel
    {
        #region Implemented Properties
        
        public string ID { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public DataFlow DataFlow { get; set; }
        public Role Role { get; set; }
        
        #endregion
        
        #region Implemented Methods
        
        public IVirtualSession CreateSession(int sliderIndex)
        {
            return new DeviceSession(sliderIndex, ID, Name, Guid.NewGuid());
        }
        
        #endregion
    }
    
    public class AudioSessionModel : ISessionModel
    {
        #region Implemented Properties
        
        public string ID { get; set; }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        
        #endregion
        
        #region Implemented Methods
        
        public IVirtualSession CreateSession(int sliderIndex)
        {
            return new VirtualSession(sliderIndex, ID, Name, Guid.NewGuid());
        }
        
        #endregion
    }
    
}