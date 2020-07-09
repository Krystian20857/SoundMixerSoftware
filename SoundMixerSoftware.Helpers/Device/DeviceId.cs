using System;
using System.Linq;
using System.Runtime.InteropServices;
using NAudio.MediaFoundation;
using SoundMixerSoftware.Common.Utils;

namespace SoundMixerSoftware.Helpers.Device
{
    public class DeviceId : IEquatable<DeviceId>, IFormattable
    {
        #region Constant

        public const int ID_LENGTH = 6;
        
        #endregion
        
        #region Private Fields
        
        private byte[] _idArray = new byte[ID_LENGTH];
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Get deviceId as byte array.
        /// </summary>
        public byte[] Bytes => _idArray;
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Create deviceId from byte array.
        /// </summary>
        /// <param name="array"></param>
        public DeviceId(byte[] array)
        {
            Array.Copy(array,0, _idArray, 0, array.Length);
        }

        public DeviceId(string input) : this(Parse(input).Bytes)
        {
            
        }

        public DeviceId()
        {
            Array.Clear(_idArray, 0, ID_LENGTH);
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Get byte from id.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte GetByte(int index)
        {
            if (index >= ID_LENGTH)
                ThrowOutOfRange(index);
            return _idArray[index];
        }

        /// <summary>
        /// Set index of byte.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetByte(int index, byte value)
        {

            if(index >= ID_LENGTH)
                ThrowOutOfRange(index);
            _idArray[index] = value;
        }

        /// <summary>
        /// Try get byte array from byte string terminated by '-' char.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out DeviceId result)
        {
            result = new DeviceId();
            if (string.IsNullOrEmpty(input))
                return false;
            var stringBytes = input.Split('-');
            var byteArray = new byte[stringBytes.Length];
            for (var n = 0; n < byteArray.Length; n++)
                byteArray[n] = Convert.ToByte(stringBytes[n], 16);
            result = new DeviceId(byteArray);
            return true;
        }

        public static DeviceId Parse(string input)
        {
            return TryParse(input, out var result) ? result : throw new ArgumentException("Cannot parse input string.");
        }

        /// <summary>
        /// Check if deviceId is empty.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static bool IsEmpty(DeviceId deviceId)
        {
            if (deviceId == null)
                return false;
            return deviceId.Bytes.All(x => x == 0x00);
        }

        #endregion
        
        #region Private Methods

        private void ThrowOutOfRange(int index) => throw new ArgumentOutOfRangeException($"Index id bigger than device id size: {index}");

        
        #endregion
        
        #region Implemented Methods
        

        public bool Equals(DeviceId other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other == null && IsEmpty(this) || IsEmpty(this) && IsEmpty(other))
                return true;
            if (other == null)
                return false;
            var otherBytes = other.Bytes;
            for(var n = 0; n < ID_LENGTH; n++)
                if (_idArray[n] != otherBytes[n])
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            var result = _idArray.Length;
            for (var n = 0; n < _idArray.Length; ++n)
                result = unchecked(result * 314159 + _idArray[n]);
            return result;
        }

        public string ToString(string format, IFormatProvider formatProvider) => ArrayUtils.ConvertToHexString(_idArray);

        public override string ToString() => ArrayUtils.ConvertToHexString(_idArray);

        #endregion
        
        #region Overriden Operators

        public static implicit operator DeviceId(string input) => new DeviceId(input);

        #endregion
    }
}