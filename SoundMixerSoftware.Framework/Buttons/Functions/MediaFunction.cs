using System;
using System.Collections.Generic;
using System.Windows.Media;
using SoundMixerSoftware.Common.Extension;
using SoundMixerSoftware.Common.Utils.EnumUtils;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Helpers.Buttons.Functions
{
    public class MediaFunction : IButton
    {
        #region Constant

        public const string MEDIA_TASK_KEY = "media";
        
        #endregion
        
        #region Private Fields

        private string _name;
        
        #endregion

        #region Properties

        public MediaTask MediaTask { get; }

        #endregion
        
        #region Implemented Properties

        public string Name
        {
            get
            {
                _name = $"Media Control: {EnumNameConverter.GetName(typeof(MediaTask),MediaTask.ToString())}";
                return _name;
            }
            set => _name = value;
        }
        public string Key { get; } = "media_func";
        public Guid UUID { get; set; }
        public ImageSource Image { get; set; } = Resource.MediaIcon.ToImageSource();
        public int Index { get; }

        #endregion
        
        #region Constructor

        public MediaFunction(int index, MediaTask mediaTask, Guid uuid)
        {
            Index = index;
            MediaTask = mediaTask;
            UUID = uuid;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(MEDIA_TASK_KEY, MediaTask);
            return result;
        }

        public void ButtonKeyDown(int index)
        {
            switch (MediaTask)
            {
                case MediaTask.Stop:
                    MediaControl.Stop();
                    break;
                case MediaTask.NextTrack:
                    MediaControl.NextTrack();
                    break;
                case MediaTask.PreviousTrack:
                    MediaControl.PrevTrack();
                    break;
                case MediaTask.PlayPause:
                    MediaControl.PauseResume();
                    break;
                case MediaTask.VolumeUp:
                    MediaControl.VolumeUp();
                    break;
                case MediaTask.VolumeDown:
                    MediaControl.VolumeDown();
                    break;
            }
        }

        public void ButtonKeyUp(int index)
        {
            
        }

        #endregion
    }
    
    public class MediaFunctionCreator : IButtonCreator
    {
        public IButton CreateButton(int index, Dictionary<object, object> container, Guid uuid)
        {
            if(!container.ContainsKey(MediaFunction.MEDIA_TASK_KEY))
                throw new NotImplementedException($"Container does not contains: {MediaFunction.MEDIA_TASK_KEY} key");
            var mediaTask = container[MediaFunction.MEDIA_TASK_KEY].ToString();
            var mediaTaskEnum = EnumUtil.Parse<MediaTask>(mediaTask);
            return new MediaFunction(index, mediaTaskEnum, uuid);
        }
    }
    
    public enum MediaTask{
        [ValueName("Play/Pause Track")]PlayPause,
        [ValueName("Play Next Track")]NextTrack,
        [ValueName("Play Previous Track")]PreviousTrack,
        [ValueName("Stop Track")]Stop,
        [ValueName("Turn Volume Up")]VolumeUp,
        [ValueName("Turn Volume Down")]VolumeDown
    }
}