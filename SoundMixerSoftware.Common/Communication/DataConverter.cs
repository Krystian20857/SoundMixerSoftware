using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NLog;
using SoundMixerSoftware.Common.Utils;

namespace SoundMixerSoftware.Common.Communication
{
    /// <summary>
    /// Converts bytes to structs
    /// </summary>
    public class DataConverter : IDisposable
    {
        #region Logger

        /// <summary>
        /// Use for logging in current class.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Private Fields
        
        /// <summary>
        /// Store collected bytes.
        /// </summary>
        private Queue<byte> _buffer = new Queue<byte>();
        /// <summary>
        /// "StructFromBytes" method is use for converting bytes to specified type structure.
        /// </summary>
        private readonly MethodInfo _convertMethod = typeof(StructUtils).GetMethod("StructFromBytes");
        /// <summary>
        /// Stores Type assigned to specified byte command.
        /// </summary>
        private readonly Dictionary<byte, (Type Type,int Size)> _typeRegistry = new Dictionary<byte, (Type, int)>();

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets and Sets byte terminator use to parse input data.
        /// </summary>
        public byte Terminator { get; set; }
        /// <summary>
        /// Gets and Sets if buffer will be cleared after size error.
        /// </summary>
        public bool ClearOnError { get; set; }

        #endregion
        
        #region Events

        /// <summary>
        /// Event fires hen data has been processed.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        /// <summary>
        /// Event fires when size of collected data is different than 
        /// </summary>
        public event EventHandler<EventArgs> SizeError;
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Creates DataConverter instance.
        /// </summary>
        /// <param name="terminator"><see cref="Terminator"/></param>
        /// <param name="clearOnError"><see cref="ClearOnError"/></param>
        public DataConverter(byte terminator, bool clearOnError = true)
        {
            Terminator = terminator;
            ClearOnError = clearOnError;
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns type to specified command.
        /// </summary>
        /// <param name="command">Byte command.</param>
        /// <param name="type">Type.</param>
        public void RegisterType(byte command, Type type)
        {
            if (_typeRegistry.ContainsKey(command))
                _typeRegistry[command] = (type, Marshal.SizeOf(type));
            else
                _typeRegistry.Add(command, (type, Marshal.SizeOf(type)));
        }

        /// <summary>
        /// Removes command with type assigment.
        /// </summary>
        /// <param name="command">Byte command.</param>
        public void UnregisterType(byte command)
        {
            if (_typeRegistry.ContainsKey(command))
                _typeRegistry.Remove(command);
        }

        /// <summary>
        /// Process input data. On data processing end <see cref="OnDataReceived"/> fires.
        /// </summary>
        /// <param name="data"></param>
        public void ProcessData(byte[] data, object arguments)
        {
            for (var n = 0;n < data.Length; n++)
            {
                if (data[n] == Terminator)
                {
                    if(_buffer.Count == 0)
                        continue;
                    var command = _buffer.Peek();
                    if (!_typeRegistry.ContainsKey(command))
                    {
                        _buffer.Clear();
                        return;
                    }
                    var (type, size) = _typeRegistry[command];

                    if (_buffer.Count != size)
                    {
                        SizeError?.Invoke(this, new EventArgs());
                        if (ClearOnError)
                        {
                            _buffer.Clear();
                            break;
                        }
                    }

                    var genericType = _convertMethod.MakeGenericMethod(type);
                    var dataEventArgs = new DataReceivedEventArgs
                    {
                        Command = command,
                        Data = genericType.Invoke(null, new object[] {_buffer.ToArray()}),
                        Arguments = arguments
                    };
                    DataReceived?.Invoke(this, dataEventArgs);
                    _buffer.Clear();
                }
                else
                    _buffer.Enqueue(data[n]);
            }
        }
        
        #endregion
        
        #region Private Methods

        #endregion

        #region Dispose
        
        /// <summary>
        /// Dispose............
        /// </summary>
        public void Dispose()
        {
            _buffer.Clear();
            GC.SuppressFinalize(this);
        }
        
        #endregion
        
    }
}