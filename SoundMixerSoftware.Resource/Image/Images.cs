
using System.Windows.Media;

namespace SoundMixerSoftware.Resource.Image
{
    public static class Images
    {
        #region Static Fields

        public static ImageLoader ImageLoader { get; private set; }
        
        private const string DMDSKRES_DLL = @"%systemroot%\system32\dmdskres.dll";
        private const string MMRES_DLL = @"%systemroot%\system32\mmres.dll";

        #endregion
        
        #region Resources

        public static ImageSource File => ImageLoader.DefaultImage;
        
        public static ImageSource Cog => GetFromFile("Cog");
        public static ImageSource Exit => GetFromFile("Exit");
        public static ImageSource Keyboard => GetFromFile("Keyboard");
        public static ImageSource List => GetFromFile("List");
        public static ImageSource Media => GetFromFile("Media");
        public static ImageSource Mute => GetFromFile("Mute");
        public static ImageSource Profile => GetFromFile("Profile");
        public static ImageSource SpeakerMute => GetFromFile("SpeakerMute");
        public static ImageSource Speaker => GetFromFile("Speaker");

        public static ImageSource FailedEmbed => GetFormEmbed(DMDSKRES_DLL, 5);
        public static ImageSource SpeakerEmbed => GetFormEmbed(MMRES_DLL, 1);
        public static ImageSource MicEmbed => GetFormEmbed(MMRES_DLL, 5);

        #endregion
        
        #region Static Methods
        
        public static void Initialize(string imagesPath)
        {
            ImageLoader = new ImageLoader(imagesPath);
        }

        public static ImageSource GetFromFile(string file)
        {
            return ImageLoader.GetOrLoad(ImageLoader.FILE_PREFIX + file);
        }

        public static ImageSource GetFormEmbed(string file, int index)
        {
            return ImageLoader.GetOrLoad(ImageLoader.EMBED_PREFIX + $"{file},{index}");
        }
        
        #endregion
    }
}