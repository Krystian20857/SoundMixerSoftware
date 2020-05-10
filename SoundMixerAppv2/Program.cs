using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using NLog;
using NLog.LayoutRenderers;
using SoundMixerAppv2.Common.Communication;
using SoundMixerAppv2.Common.Communication.Serial;
using SoundMixerAppv2.Common.Config;
using SoundMixerAppv2.Common.Config.Yaml;
using SoundMixerAppv2.Common.LocalSystem;
using SoundMixerAppv2.Common.Logging;
using SoundMixerAppv2.LocalSystem;
using SoundMixerAppv2.Win32.USBLib;
using SoundMixerAppv2.Win32.Win32;
using SoundMixerAppv2.Win32.Wrapper;

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
        private static SerialConnection connection;
        private static bool _micmute = false;
        private static bool _speakermute = false;
        public static USBDevice _usbDevice = new USBDevice(NativeClasses.GUID_DEVINTERFACE.GUID_DEVINTERFACE_PARALLEL);
        //public static USBDevice _usbDevice = new USBDevice();
        [STAThread]
        public static void Main(string[] args)
        {
            _usbDevice.VID = 0x468F;
            _usbDevice.PID = 0x895D;
            
            LoggerUtils.SetupLogger(LocalContainer.LogsFolder);
            LocalManager.ResolveLocal();
            
            connection = new SerialConnection(new SerialConfig
            {
                BaudRate = 115200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                Encoding = Encoding.UTF8
            });
            
            converter.DataReceived += ConverterOnDataReceived;
            converter.RegisterType(0x01, typeof(SliderStruct));
            converter.RegisterType(0x02, typeof(ButtonStruct));
            converter.RegisterType(0x03, typeof(DeviceResponse));
            
            connection.DeviceConnected += ConnectionOnDeviceConnected;
            connection.DataReceived += ConnectionOnDataReceived;
            connection.Connect(COMPORT);

            Console.WriteLine(Marshal.SizeOf(typeof(SliderStruct)));
            
            _usbDevice.DeviceArrive += UsbDeviceOnDeviceArrive;
            _usbDevice.DeviceRemove += UsbDeviceOnDeviceRemove;

            
            var windowThread = new Thread(() =>
            {
                new NativeWindowHandler();
            });
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.IsBackground = true;
            windowThread.Start();

            do
            {
                
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        private static void UsbDeviceOnDeviceRemove(object sender, DeviceStateArgs e)
        {
            Console.WriteLine($"Device Removed: {e.DeviceProperties.COMPort}");
        }

        private static void UsbDeviceOnDeviceArrive(object sender, DeviceStateArgs e)
        {
            Console.WriteLine($"Device Arrived: {e.DeviceProperties.COMPort}");
        }

        private static string COMPORT = "COM10";  

        private static void ConverterOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Command == 0x01)
            {
                SliderStruct data = e.Data;
                Console.WriteLine($"Command: {data.Command}\n\t Slider: {data.Slider}\n\t Value: {data.Value}");
            }
            else if (e.Command == 0x02)
            {
                ButtonStruct data = e.Data;
                if (data.State != 1)
                    return;
                Console.WriteLine($"Command: {data.Command}\n\t Button: {data.Button}\n\t State: {data.State}");
                switch (data.Button)
                {
                    case 0:
                        _speakermute = !_speakermute;
                        WriteLed(0,(byte)(_speakermute ? 1 : 0));
                        break;
                    case 1:
                        _micmute = !_micmute;
                        WriteLed(1,(byte)(_micmute ? 1 : 0));
                        break;
                    case 2:
                        MediaControl.PrevTrack();
                        break;
                    case 3:
                        MediaControl.PauseResume();
                        break;
                    case 4:
                        MediaControl.NextTrack();
                        break;
                }
            }
            else if (e.Command == 0x03)
            {
                DeviceResponse response = e.Data;
                Console.WriteLine($"Device name: {response.name}"); 
                Console.WriteLine($"Device uuid: {BitConverter.ToString(response.uuid)}");
            }
        }

        private static void WriteLed(byte led, byte state)
        {
            connection.SendData(COMPORT, new LedStruct
            {
                Led = led,
                Command = 0x01,
                State = state,
            });
            connection.SendBytes(COMPORT, new byte[]{0xFF});
        }

        private static void ConnectionOnDataReceived(object sender, SerialDataReceivedArgs e)
        {
            converter.ProcessData(e.Data);
        }

        private static void ConnectionOnDeviceConnected(object sender, DeviceStateChangeArgs e)
        {
            Console.WriteLine($"Connected to device: {e.COMPort}");
            connection.SendBytes(COMPORT,new[] {(byte)0x02});
            connection.SendBytes(COMPORT, new byte[]{0xFF});
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct SliderStruct
        {
            [FieldOffset(0)] public byte Command;
            [FieldOffset(1)] public byte Slider;
            [FieldOffset(2)] public short Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ButtonStruct
        {
            [FieldOffset(0)] public byte Command;
            [FieldOffset(1)] public byte Button;
            [FieldOffset(2)] public byte State;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct LedStruct
        {
            [FieldOffset(0)] public byte Command;
            [FieldOffset(1)] public byte Led;
            [FieldOffset(2)] public byte State;
        }

        
        [StructLayout(LayoutKind.Sequential)]
        private struct DeviceResponse
        {
            [MarshalAs(UnmanagedType.U1)] public byte command;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]public byte[] uuid;
        }

        private class NativeWindowHandler : NativeWindow
        {
            public NativeWindowHandler()
            {
                CreateHandle(new CreateParams());
                Program._usbDevice.RegisterDeviceChange(this.Handle);
                Application.Run();
            }
            
            [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
            protected override void WndProc(ref Message m)
            {
                Program._usbDevice.ProcessMessage((uint) m.Msg, m.WParam, m.LParam);
                base.WndProc(ref m);
            }
        }

        #endregion
    }
}
