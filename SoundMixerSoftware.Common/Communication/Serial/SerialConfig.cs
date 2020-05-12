using System.IO.Ports;
using System.Text;

namespace SoundMixerSoftware.Common.Communication.Serial
{
    public class SerialConfig
    {
        #region Public Properties
        /// <summary>
        /// Defines speed of serial port.
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// Defines frame data length.
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// Defines stop bits.
        /// </summary>
        public StopBits StopBits { get; set; }
        /// <summary>
        /// Defines parity bits type.
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// Defines default encoding.
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// Optional serial port name.
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// Defines timeout time :0.
        /// </summary>
        public int Timeout { get; set; }

        #endregion

        #region Constructor
        
        public SerialConfig()
        {
            
        }

        public SerialConfig(int baudRate, int dataBits, StopBits stopBits, Parity parity, Encoding encoding, int timeout)
        {
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            Encoding = encoding;
            Timeout = timeout;
        }

        public SerialConfig(int baudRate, int dataBits, StopBits stopBits, Parity parity, Encoding encoding, string portName, int timeout)
        {
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            Encoding = encoding;
            PortName = portName;
            Timeout = timeout;
        }

        #endregion
        
        #region Public Static Methods
        
        public static SerialPort SetupSerialPort(SerialConfig config)
        {
            var serialPort =  new SerialPort()
            {
                BaudRate = config.BaudRate,
                DataBits = config.DataBits,
                StopBits = config.StopBits,
                Parity = config.Parity,
                Encoding = config.Encoding,
            };
            if (!string.IsNullOrWhiteSpace(config.PortName))
                serialPort.PortName = config.PortName;
            return serialPort;
        }

        #endregion
    }
}