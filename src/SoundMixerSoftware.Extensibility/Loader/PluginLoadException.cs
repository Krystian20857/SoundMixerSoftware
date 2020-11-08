using System;

namespace SoundMixerSoftware.Extensibility.Loader
{
    public class PluginLoadException : Exception
    {
        public PluginLoadException(string message) : base(message)
        {
            
        }   
    }
}