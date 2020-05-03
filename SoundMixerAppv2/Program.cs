using System;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NLog;
using SoundMixerAppv2.Common.Communication;
using SoundMixerAppv2.Common.Communication.Serial;
using SoundMixerAppv2.Common.Config;
using SoundMixerAppv2.Common.Config.Yaml;
using SoundMixerAppv2.Common.LocalSystem;
using SoundMixerAppv2.Common.Logging;
using SoundMixerAppv2.LocalSystem;

namespace SoundMixerAppv2
{
    public static class Program
    {
        #region Logger
        
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        #endregion
        
        #region Public Properites
        
        public static LocalManager LocalManager { get; } = new LocalManager(typeof(LocalContainer));

        #endregion
        
        #region Main Method

        private static DataConverter converter = new DataConverter(0xFF);
        public static void Main(string[] args)
        {
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            LocalManager.ResolveLocal();
            
            var connection = new SerialConnection(new SerialConfig
            {
                BaudRate = 115200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                Encoding = Encoding.UTF8
            });
            
            converter.OnDataReceived += ConverterOnOnDataReceived;
            converter.RegisterType(0x01, typeof(SliderStruct));
            
            connection.OnDeviceConnected += ConnectionOnOnDeviceConnected;
            connection.OnDataReceived += ConnectionOnOnDataReceived;
            connection.Connect("COM7");
            
            Console.WriteLine(Marshal.SizeOf(typeof(SliderStruct)));

            do
            {
                
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        private static void ConverterOnOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Command == 0x01)
            {
                SliderStruct data = e.Data;
                Console.WriteLine($"Command: {data.Command}\n\t Slider: {data.Slider}\n\t Value: {data.Value}");
            }
        }

        private static void ConnectionOnOnDataReceived(object sender, SerialDataReceivedArgs e)
        {
            converter.ProcessData(e.Data);
        }

        private static void ConnectionOnOnDeviceConnected(object sender, DeviceStateChangeArgs e)
        {
            Console.WriteLine($"Connected to device: {e.COMPort}");
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct SliderStruct
        {
            [FieldOffset(0)] public byte Command;
            [FieldOffset(1)] public byte Slider;
            [FieldOffset(2)] public short Value;
        }

        #endregion
    }
}
