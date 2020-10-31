using System;
using System.Threading.Tasks;
using NLog;

namespace SoundMixerSoftware.Framework.Device
{
    public static class DeviceNotifier
    {
        #region Current Class Logger

        /// <summary>
        /// Current Class Logger.
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion

        #region Private Fields

        private static Task _testDeviceTask = Task.CompletedTask;
        
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Lights devices buttons.
        /// </summary>
        /// <param name="comport"></param>
        public static void TestLights(string comport)
        {
            //if (_testDeviceTask.Status == TaskStatus.RanToCompletion)
            _testDeviceTask = Task.Run(async () =>
            {
                if (DeviceHandlerGlobal.ConnectedDevice.TryGetValue(comport, out var result))
                {
                    var deviceHandler = DeviceHandlerGlobal.Instance;
                    var buttons = result.DeviceResponse.button_count;
                    for (byte n = 0; n < buttons; n++)
                    {
                        LightButton(comport, n, true);
                        await Task.Delay(500);
                        LightButton(comport, n, false);
                        await Task.Delay(500);
                    }
                }
            });
        }

        /// <summary>
        /// Light button for specified period of time.
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="button"></param>
        /// <param name="illuminationTime"></param>
        private static void LightButton(string comport, byte button, TimeSpan illuminationTime)
        {
            Task.Factory.StartNew(async () =>
            {
                LightButton(comport, button, true);
                await Task.Delay(illuminationTime);
                LightButton(comport, button, false);
            });
        }
        
        /// <summary>
        /// Light button for specified period of time.
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="button"></param>
        /// <param name="illuminationTime"></param>
        public static void LightButton(byte button, TimeSpan illuminationTime)
        {
            Task.Factory.StartNew(async () =>
            {
                LightButton(button, true);
                await Task.Delay(illuminationTime);
                LightButton(button, false);
            });
        }
        
        /// <summary>
        /// Light button for specified period of time on every device.
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="button"></param>
        /// <param name="illuminationTime"></param>
        public static void LightButton(byte button, bool state)
        {
            foreach (var device in DeviceHandlerGlobal.ConnectedDevice)
            {
                var deviceId = new DeviceId(device.Value.DeviceResponse.uuid);
                var buttonOffset = DeviceHandlerGlobal.ButtonOffsetManager.GetOrCreateOffset(deviceId);
                var buttonIndex = button - buttonOffset;
                if (buttonIndex >= 0)
                    LightButton(device.Key, (byte)buttonIndex, state);
            }
        }

        /// <summary>
        /// Set illumination state of specified button.
        /// </summary>
        /// <param name="comport">serial port of device</param>
        /// <param name="button">button index</param>
        /// <param name="state">led state</param>
        private static void LightButton(string comport, byte button, bool state)
        {
            var structure = new LedStruct()
            {
                command = Command.LED_COMMAND,
                led = button,
                state = state ? LedState.HIGH : LedState.LOW
            };
            DeviceHandlerGlobal.Instance.SendData(comport, structure);
        }

        #endregion
    }
}