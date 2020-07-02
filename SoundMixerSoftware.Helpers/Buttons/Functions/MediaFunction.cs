using System;
using System.Collections.Generic;
using System.Windows.Input;
using SoundMixerSoftware.Common.Utils.Enum;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Helpers.Buttons.Functions
{
    public class MediaFunction : IButton
    {
        #region Constant

        public const string MEDIA_TASK_KEY = "media";
        
        #endregion
        
        #region Properties

        public MediaTask MediaTask { get; }

        #endregion
        
        #region Implemented Properties

        public string Name { get; set; } = "Button Media Controller";
        public string Key { get; } = "media_func";
        public Dictionary<object, object> Container { get; }
        
        #endregion
        
        #region Constructor

        public MediaFunction(MediaTask mediaTask)
        {
            MediaTask = mediaTask;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(MEDIA_TASK_KEY, MediaTask);
            return result;
        }

        public void ButtonPressed(int index)
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
            }
        }

        #endregion
    }
    
    public class MediaFunctionCreator : IButtonCreator
    {
        public IButton CreateButton(Dictionary<object, object> container)
        {
            if(!container.ContainsKey(MediaFunction.MEDIA_TASK_KEY))
                throw new NotImplementedException($"Container does not contains: {MediaFunction.MEDIA_TASK_KEY} key");
            var mediaTask = container[MediaFunction.MEDIA_TASK_KEY].ToString();
            var mediaTaskEnum = Enum.TryParse<MediaTask>(mediaTask, out var result) ? result : default;
            return new MediaFunction(mediaTaskEnum);
        }
    }
    
    public enum MediaTask{
        [ValueName("Play/Pause Track")]PlayPause,
        [ValueName("Play Next Track")]NextTrack,
        [ValueName("Play Previous Track")]PreviousTrack,
        [ValueName("Stop Track")]Stop
    }
}