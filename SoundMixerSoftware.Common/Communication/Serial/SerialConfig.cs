using System.IO.Ports;
using System.Text;
using YamlDotNet.Serialization;

namespace SoundMixerSoftware.Common.Communication.Serial
{
    public class SerialConfig
    {
        #region Nullable Properties

        [YamlMember(Alias = nameof(BaudRate))]
        public int? BaudRateNullable { get; set; }
        [YamlMember(Alias = nameof(DataBits))]
        public int? DataBitsNullable { get; set; }
        [YamlMember(Alias = nameof(StopBits))]
        public StopBits? StopBitsNullable { get; set; }
        [YamlMember(Alias = nameof(Parity))]
        public Parity? ParityNullable { get; set; }
        [YamlMember(Alias = nameof(Timeout))]
        public int? TimeoutNullable { get; set; }
        [YamlMember(Alias = nameof(RtsEnable))]
        public bool? RtsEnableNullable { get; set; }
        [YamlMember(Alias = nameof(DtsEnable))]
        public bool? DtsEnableNullable { get; set; }

        #endregion

        #region Base Properties

        /// <summary>
        /// Defines speed of serial port.
        /// </summary>
        [YamlIgnore]
        public int BaudRate
        {
            get => BaudRateNullable ?? default;
            set => BaudRateNullable = value;
        }

        /// <summary>
        /// Defines frame data length.
        /// </summary>
        [YamlIgnore]
        public int DataBits
        {
            get => DataBitsNullable ?? default;
            set => DataBitsNullable = value;
        }

        /// <summary>
        /// Defines stop bits.
        /// </summary>
        [YamlIgnore]
        public StopBits StopBits
        {
            get => StopBitsNullable ?? default;
            set => StopBitsNullable = value;
        }

        /// <summary>
        /// Defines parity bits type.
        /// </summary>
        [YamlIgnore]
        public Parity Parity
        {
            get => ParityNullable ?? default;
            set => ParityNullable = value;
        }

        /// <summary>
        /// Optional serial port name.
        /// </summary>
        [YamlIgnore]
        public string PortName { get; set; }

        /// <summary>
        /// Defines timeout time :0.
        /// </summary>
        [YamlIgnore]
        public int Timeout
        {
            get => TimeoutNullable ?? default;
            set => TimeoutNullable = value;
        }

        /// <summary>
        /// Enable RTS flag.
        /// </summary>
        [YamlIgnore]
        public bool RtsEnable
        {
            get => RtsEnableNullable ?? default;
            set => RtsEnableNullable = value;
        }

        /// <summary>
        /// Enable DTR flag.
        /// </summary>
        [YamlIgnore]
        public bool DtsEnable
        {
            get => DtsEnableNullable ?? default;
            set => DtsEnableNullable = value;
        }

        #endregion

        #region Constructor

        public SerialConfig()
        {
        }

        public SerialConfig(int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout)
        {
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            Timeout = timeout;
        }

        public SerialConfig(int baudRate, int dataBits, StopBits stopBits, Parity parity, string portName, int timeout)
        {
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            PortName = portName;
            Timeout = timeout;
        }

        #endregion

        #region Public Static Methods

        public static SerialPort SetupSerialPort(SerialConfig config)
        {
            var serialPort = new SerialPort
            {
                BaudRate = config.BaudRate,
                DataBits = config.DataBits,
                StopBits = config.StopBits,
                Parity = config.Parity,
                DtrEnable = config.DtsEnable,
                RtsEnable = config.RtsEnable
            };
            if (!string.IsNullOrWhiteSpace(config.PortName))
                serialPort.PortName = config.PortName;
            return serialPort;
        }

        #endregion
    }
}