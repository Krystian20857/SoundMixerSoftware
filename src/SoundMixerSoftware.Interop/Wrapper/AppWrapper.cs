using System;
using System.Runtime.InteropServices;
using SoundMixerSoftware.Interop.Constant;
using SoundMixerSoftware.Interop.Interface;
using SoundMixerSoftware.Interop.Method;

namespace SoundMixerSoftware.Interop.Wrapper
{
    public static class AppWrapper
    {
        /// <summary>
        /// Get application name from apps folder.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string GetAppName(uint pid)
        {
            try
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var appResolver = (IApplicationResolver) new ApplicationResolver();
                appResolver.GetAppIDForProcess(pid, out var appId, out var _, out var __, out var ___);
                Marshal.ReleaseComObject(appResolver);

                var shellItem = Shell32.SHCreateItemInKnownFolder(FolderId.AppsFolderUUID, KF.KF_FLAG_DONT_VERITY, appId, typeof(IShellItem2).GUID);
                return shellItem.GetString(ref PropertyKeys.PKEY_ItemNameDisplay);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        
    }
}