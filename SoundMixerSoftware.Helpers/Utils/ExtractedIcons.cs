using System.Drawing;
using SoundMixerSoftware.Win32.Wrapper;

namespace SoundMixerSoftware.Helpers.Utils
{
    public class ExtractedIcons
    {
        #region Const DLLs
        
        public const string DMDSKRES_DLL = @"%systemroot%\system32\dmdskres.dll";
        public const string MMRES_DLL = @"%systemroot%\system32\mmres.dll";

        #endregion

        public static Icon FailedIcon = IconExtractor.ExtractIcon(DMDSKRES_DLL, 5, true);
        public static Icon SpeakerIcon = IconExtractor.ExtractIcon(MMRES_DLL, 1, true);
        public static Icon MicIcon = IconExtractor.ExtractIcon(MMRES_DLL, 5, true);
    }
}