using System;
using System.Collections.Generic;

namespace SoundMixerSoftware.Extensibility.Storage
{
    public interface IPluginInfoManager
    {
        Dictionary<string, PluginInfo> Storage { get; }
        PluginInfo GetInfo(string folderName);
        void SetInfo(string folderName, PluginInfo pluginInfo);
        void LoadAll();
    }
}