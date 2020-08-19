using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NLog;
using SoundMixerSoftware.Win32.Interop.Enum;
using SoundMixerSoftware.Win32.Interop.Method;
using SoundMixerSoftware.Win32.Interop.Struct;

namespace SoundMixerSoftware.Win32.USBLib
{
    public static class USBDescriptor
    {
        #region Logger
    
        /// <summary>
        /// Current Class Logger.
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
        #endregion
        
        #region Const
        
        /// <summary>
        /// Data buffer size.
        /// </summary>
        public const int BUFFER_SIZE = 1024;
        
        #endregion
        
        #region Public Static Methods

        /// <summary>
        /// Get USB devices descriptors. 
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static IEnumerable<DeviceProperties> GetDescriptors(IEnumerable<USBID> vidpid)
        {
            return vidpid.SelectMany(entry => GetDescriptors(entry.Vid, entry.Pid));
        }

        /// <summary>
        /// Get USB devices descriptors.
        /// </summary>
        /// <param name="vid">vendor id</param>
        /// <param name="pid">product id</param>
        /// <returns></returns>
        public static IEnumerable<DeviceProperties> GetDescriptors(uint vid, uint pid)
        {
            var deviceHandle = Setupapi.SetupDiGetClassDevs(IntPtr.Zero, "USB", IntPtr.Zero,
                (uint) (DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_ALLCLASSES));
            var bufferPtr = Marshal.AllocHGlobal(BUFFER_SIZE);
            if (deviceHandle != new IntPtr(-1))
            {
                var hardwareIDFormat = FormatHardwareID(vid, pid);
                var success = true;
                var index = 0;
                while (success)
                {
                    var devinfo = new SP_DEVINFO_DATA();
                    var lastError = 0;
                    devinfo.cbSize = (uint) Marshal.SizeOf(devinfo);
                    success = Setupapi.SetupDiEnumDeviceInfo(deviceHandle, (uint) index, ref devinfo);
                    if (success)
                    {
                        Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle, ref devinfo,
                            (uint) SPDRP.SPDRP_HARDWAREID, out var regType, IntPtr.Zero, 0,
                            out var reqSize);
                        lastError = Marshal.GetLastWin32Error();
                        if ((WinErrors) lastError == WinErrors.ERROR_INSUFFICIENT_BUFFER)
                        {
                            if (reqSize <= BUFFER_SIZE)
                            {
                                if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle, ref devinfo,
                                    (int) SPDRP.SPDRP_HARDWAREID, out regType, bufferPtr, BUFFER_SIZE,
                                    out reqSize))
                                {
                                    var hardwareID = Marshal.PtrToStringAnsi(bufferPtr);
                                    if (hardwareID.StartsWith(hardwareIDFormat))
                                    {
                                        var devproperties = new DeviceProperties();
                                        devproperties.Vid = vid;
                                        devproperties.Pid = pid;
                                        if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle,
                                            ref devinfo,
                                            (int) SPDRP.SPDRP_FRIENDLYNAME, out regType, bufferPtr,
                                            BUFFER_SIZE,
                                            out reqSize))
                                        {
                                            devproperties.FriendlyName = Marshal.PtrToStringAnsi(bufferPtr);
                                        }

                                        if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle,
                                            ref devinfo,
                                            (int) SPDRP.SPDRP_DEVTYPE, out regType, bufferPtr,
                                            BUFFER_SIZE,
                                            out reqSize))
                                        {
                                            devproperties.DeviceType = Marshal.PtrToStringAnsi(bufferPtr);
                                        }

                                        if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle,
                                            ref devinfo,
                                            (int) SPDRP.SPDRP_MFG, out regType, bufferPtr,
                                            BUFFER_SIZE,
                                            out reqSize))
                                        {
                                            devproperties.DeviceManufacturer = Marshal.PtrToStringAnsi(bufferPtr);
                                        }

                                        if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle,
                                            ref devinfo,
                                            (int) SPDRP.SPDRP_LOCATION_INFORMATION, out regType,
                                            bufferPtr,
                                            BUFFER_SIZE,
                                            out reqSize))
                                        {
                                            devproperties.DeviceLocation = Marshal.PtrToStringAnsi(bufferPtr);
                                        }

                                        if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle,
                                            ref devinfo,
                                            (int) SPDRP.SPDRP_LOCATION_PATHS, out regType, bufferPtr,
                                            BUFFER_SIZE,
                                            out reqSize))
                                        {
                                            devproperties.DevicePath = Marshal.PtrToStringAnsi(bufferPtr);
                                        }

                                        if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle,
                                            ref devinfo,
                                            (int) SPDRP.SPDRP_PHYSICAL_DEVICE_OBJECT_NAME, out regType,
                                            bufferPtr,
                                            BUFFER_SIZE,
                                            out reqSize))
                                        {
                                            devproperties.DevicePhysicalObjectName =
                                                Marshal.PtrToStringAnsi(bufferPtr);
                                        }

                                        if (Setupapi.SetupDiGetDeviceRegistryProperty(deviceHandle,
                                            ref devinfo,
                                            (int) SPDRP.SPDRP_DEVICEDESC, out regType, bufferPtr,
                                            BUFFER_SIZE,
                                            out reqSize))
                                        {
                                            devproperties.DeviceDescription = Marshal.PtrToStringAnsi(bufferPtr);
                                        }

                                        var deviceRegKey = Setupapi.SetupDiOpenDevRegKey(deviceHandle,
                                            ref devinfo, (int) DICS_FLAG.DICS_FLAG_GLOBAL, 0,
                                            (int) DIREG.DIREG_DEV,
                                            (int) REGKEYSECURITY.KEY_READ);
                                        if ((WinErrors) deviceRegKey ==
                                            WinErrors.ERROR_INVALID_HANDLE)
                                        {
                                            lastError = Marshal.GetLastWin32Error();
                                            break;
                                        }

                                        var data = new StringBuilder(BUFFER_SIZE);
                                        var size = (uint) data.Capacity;
                                        var result = Advapi.RegQueryValueEx(deviceRegKey, "PortName", 0,
                                            out var type, data, ref size);
                                        if ((WinErrors) result == WinErrors.ERROR_SUCCESS)
                                        {
                                            devproperties.COMPort = data.ToString();
                                        }

                                        Advapi.RegCloseKey(deviceRegKey);
                                        yield return devproperties;
                                    }
                                }
                            }

                        }
                        else
                            lastError = Marshal.GetLastWin32Error();
                    }
                    index++;
                }
            }
            else
                Logger.Warn($"Win32 Error: {Marshal.GetLastWin32Error()}");

            Setupapi.SetupDiDestroyDeviceInfoList(deviceHandle);
            Marshal.FreeHGlobal(bufferPtr);
            //yield return default;
        }

        /// <summary>
        /// Make hardware ID from vid na pid numbers.
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string FormatHardwareID(uint vid, uint pid)
        {
            var vidString = vid.ToString("X4");
            var pidString = pid.ToString("X4");
            return $"USB\\VID_{vidString}&PID_{pidString}";
        }
    }
    
    #endregion

    public struct DeviceProperties : ICloneable
    {
        /// <summary>
        /// Friendly name appears in Device Manager.
        /// </summary>
        public string FriendlyName { get; set; }
        /// <summary>
        /// Description of device.
        /// </summary>
        public string DeviceDescription { get; set; }
        /// <summary>
        /// Device type: scanners, printers, serial, etc.
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// Manufacturer of device.
        /// </summary>
        public string DeviceManufacturer { get; set; }
        /// <summary>
        /// Device class.
        /// </summary>
        public string DeviceClass { get; set; }
        /// <summary>
        /// Device location information.
        /// </summary>
        public string DeviceLocation { get; set; }
        /// <summary>
        /// Device location.
        /// </summary>
        public string DevicePath { get; set; }
        /// <summary>
        /// Device physical object name.
        /// </summary>
        public string DevicePhysicalObjectName { get; set; }
        /// <summary>
        /// When device class in serial when <<see cref="COMPort"> is not null.
        /// </summary>
        public string COMPort { get; set; }
        /// <summary>
        /// Vendor id of device.
        /// </summary>
        public uint Vid { get; set; }
        /// <summary>
        /// Product id of device.
        /// </summary>
        public uint Pid { get; set; }

        /// <summary>
        /// Short device properties as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach(var property in typeof(DeviceProperties).GetProperties())
            {
                stringBuilder.Append(property.Name);
                stringBuilder.Append(" = ");
                stringBuilder.Append(property.GetValue(this));
                stringBuilder.Append("\t\n");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Shallow copy object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}