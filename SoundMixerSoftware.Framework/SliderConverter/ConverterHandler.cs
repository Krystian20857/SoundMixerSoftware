using System;
using System.Collections.Generic;
using NLog;
using SoundMixerSoftware.Framework.Device;
using SoundMixerSoftware.Framework.Profile;
using SoundMixerSoftware.Framework.SliderConverter.Converters;

namespace SoundMixerSoftware.Framework.SliderConverter
{
    public static class ConverterHandler
    {
        #region Private fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Public Properties
        
        public static Dictionary<string, IConverterCreator> ConverterRegistry { get; } = new Dictionary<string, IConverterCreator>();
        public static List<List<IConverter>> Converters { get; } = new List<List<IConverter>>();
        public static List<ConverterStruct> GlobalConverters { get; } = new List<ConverterStruct>();

        #endregion
        
        #region Evetns

        public static event EventHandler<ConverterArgs> ConverterAdded;
        public static event EventHandler<ConverterArgs> ConverterRemoved;
        
        #endregion
        
        #region Constructor

        #endregion

        #region Public Static Methods

        public static void CreateConverters()
        {
            Converters.Clear();
            var sliderCount = ProfileHandler.SelectedProfile.SliderCount;
            Converters.Capacity = sliderCount;
            for (var n = 0; n < sliderCount; n++)
                Converters.Add(new List<IConverter>());
            var sliders = ProfileHandler.SelectedProfile.Sliders;
            for (var n = 0; n < sliders.Count; n++)
            {
                var sliderStruct = sliders[n];
                if(n >= sliderCount)
                    continue;
                if(sliderStruct.Converters == null)
                    sliderStruct.Converters = new List<ConverterStruct>();
                for (var x = 0; x < GlobalConverters.Count; x++)
                {
                    var converter = GlobalConverters[x];
                    AddConverter(n, converter);
                }
                for (var x = 0; x < sliderStruct.Converters.Count; x++)
                {
                    var converter = sliderStruct.Converters[x];
                    AddConverter(n, converter);
                }
            }
        }

        public static void RegisterCreator(string key, IConverterCreator creator)
        {
            if (!ConverterRegistry.ContainsKey(key))
                ConverterRegistry.Add(key, creator);
        }

        public static void UnregisterCreator(string key)
        {
            if (ConverterRegistry.ContainsKey(key))
                ConverterRegistry.Remove(key);
        }

        public static IConverter AddConverter(int index, ConverterStruct converterStruct)
        {
            var key = converterStruct.Key;
            if (string.IsNullOrEmpty(key) || !ConverterRegistry.ContainsKey(key))
                return null;
            var creator = ConverterRegistry[key];
            var converter = (IConverter) null;
            try
            {
                converter = creator.CreateConverter(index, converterStruct.Container, converterStruct.UUID);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }

            Converters[index].Add(converter);
            ConverterAdded?.Invoke(null, new ConverterArgs(index, Converters[index].IndexOf(converter), converter));
            return converter;
        }

        public static ConverterStruct AddConverter(int index, IConverter converter)
        {
            converter.Index = index;
            Converters[index].Add(converter);
            ConverterAdded?.Invoke(null, new ConverterArgs(index, Converters[index].IndexOf(converter), converter));
            return new ConverterStruct
            {
                Container = converter.Save(),
                Key = converter.Key,
                UUID = converter.UUID
            };
        }

        public static void RemoveConverter(int index, int converterIndex)
        {
            var converter = Converters[index][converterIndex];
            ConverterRemoved?.Invoke(null, new ConverterArgs(index, Converters[index].IndexOf(converter), converter));
            Converters[index].Remove(converter);
        }
        
        public static void RemoveConverter(int index, IConverter converter)
        {
            ConverterRemoved?.Invoke(null, new ConverterArgs(index, Converters[index].IndexOf(converter), converter));
            Converters[index].Remove(converter);
        }

        public static bool HasConverter<T>(int index) where T : IConverter
        {
            foreach(var converter in Converters[index])
                if (converter is T)
                    return true;
            return false;
        }

        public static float ConvertValue(int index, float value, DeviceId deviceId)
        {
            var converters = Converters[index];
            if (converters.Count == 0 && GlobalConverters.Count == 0)
                return value;
            for (var n = 0; n < converters.Count; n++)
                value = converters[n].Convert(value, index, n, deviceId);
            return value;
        }
        
        
        #endregion
        
    }

    public class ConverterArgs : EventArgs
    {
        public int Index { get; set; }
        public int ConverterIndex { get; set; }
        public IConverter Converter { get; set; }

        public ConverterArgs(int index, int converterIndex, IConverter converter)
        {
            Index = index;
            ConverterIndex = converterIndex;
            Converter = converter;
        }
    }
}