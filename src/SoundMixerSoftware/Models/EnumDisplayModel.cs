using System;
using SoundMixerSoftware.Common.Utils.EnumUtils;

namespace SoundMixerSoftware.Models
{
    public class EnumDisplayModel<T> where T : struct, IConvertible
    {
        #region Private Fields

        private string _name;
        private T _enumValue;
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Defines visible name;
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Define MediaTask linked name;
        /// </summary>
        public T EnumValue
        {
            get => _enumValue;
            set
            {
                _enumValue = value;
                _name = EnumNameConverter.GetName(typeof(T), value.ToString());
            }
        }
        
        #endregion
        
        #region Constructor

        public EnumDisplayModel()
        {
            if(!typeof(T).IsEnum) throw new ArgumentException($"Type applied to {nameof(EnumDisplayModel<T>)} must be enum.");
        }
        
        #endregion
    }
}