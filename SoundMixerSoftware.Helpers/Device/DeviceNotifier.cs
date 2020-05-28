using System.Threading.Tasks;
using NLog;
using SoundMixerSoftware.Models;

namespace SoundMixerSoftware.Helpers.Device
{
    public static class DeviceNotifier
    {
        #region Current Class Logger

        /// <summary>
        /// Current Class Logger.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion

        #region Public Static Methods

        public static void TestLights(string comport)
        {
            var testTask = Task.Run(async () =>
            {
                if (DeviceHandlerGlobal.ConnectedDevice.TryGetValue(comport, out var result))
                {
                    var deviceHandler = DeviceHandlerGlobal.DeviceHandler;
                    var buttons = result.DeviceResponse.button_count;
                    for (byte n = 0; n < buttons; n++)
                    {
                        var structure = new LedStruct
                        {
                            command = 0x01,
                            led = n,
                            state = 0x01
                        };
                        deviceHandler.SendData(comport, structure);
                        await Task.Delay(500);
                        structure.state = 0x00;
                        deviceHandler.SendData(comport, structure);
                        await Task.Delay(500);
                    }
                }
            });
        }

        #endregion
    }
}