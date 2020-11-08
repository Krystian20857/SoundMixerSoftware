using System;
using System.Collections.Generic;
using SoundMixerSoftware.Framework.Device;

namespace SoundMixerSoftware.Framework.SliderConverter.Converters
{
    public class LogarithmicConverter : IConverter
    {
        #region Constant

        public const string LOG_BASE_KEY = "base";

        #endregion
        
        #region Private Fields

        private float a = float.NaN;
        private float b = float.NaN;
        
        #endregion
        
        #region Public Properties
        
        public float LogBase { get; }

        #endregion
        
        #region Implemented Properties
        
        public string Name { get; set; } = "Logarithmic Converter";
        public string Key { get; set; } = "log_converter";
        public int Index { get; set; }
        public Guid UUID { get; set; }
        
        #endregion
        
        #region Constructor

        public LogarithmicConverter(Guid uuid, float x1, float y1, float x2, float y2, float logBase)
        {
            UUID = uuid;
            LogBase = logBase;
            b = (float)Math.Log(y1 / y2, logBase) / (x1 - x2);
            a = (float)(y1 / Math.Pow(logBase, b * x1));
        }

        public LogarithmicConverter(Guid uuid, float logBase) : this(uuid, 1, 1, 100, 100, logBase)
        {
            
        }

        public LogarithmicConverter(int index, Guid uuid, float logBase) : this(uuid, logBase)
        {
            Index = index;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public Dictionary<object, object> Save()
        {
            var result = new Dictionary<object, object>();
            result.Add(LOG_BASE_KEY, LogBase);
            return result;
        }

        public float Convert(float inputValue, int sliderIndex, int converterIndex, DeviceId device)
        {
            if (inputValue < 1)
                return 0;
            return (float)(a * Math.Pow(LogBase, inputValue * b));
        }
        
        #endregion
    }
    
    public class LogConverterCreator : IConverterCreator
    {
        public IConverter CreateConverter(int index, Dictionary<object, object> container, Guid uuid)
        {
            var logBaseObject = container.ContainsKey(LogarithmicConverter.LOG_BASE_KEY) ? container[LogarithmicConverter.LOG_BASE_KEY] : Math.E;
            var logBase = float.TryParse(logBaseObject.ToString(), out var result) ? result : (float)Math.E;
            return new LogarithmicConverter(index, uuid, logBase);
        }
    }
}