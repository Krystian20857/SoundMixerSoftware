using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SoundMixerSoftware.Extensibility.Loader
{
    public class AssemblyLoader
    {
        public static IEnumerable<Assembly> LoadAssemblies(string folder)
        {
            return from file in Directory.GetFiles(folder) where Path.GetExtension(file) == ".dll" select Assembly.LoadFrom(file);
        }

        public static IEnumerable<Assembly> LoadAssemblies(string folder, IEnumerable<string> ignoreFiles)
        {
            return from file in Directory.GetFiles(folder) where Path.GetExtension(file) == ".dll" && !ignoreFiles.Contains(Path.GetFileName(file)) select Assembly.LoadFrom(file);
        }

        public static IEnumerable<Assembly> LoadAssemblyFiles(string folder, IEnumerable<string> files)
        {
            return from file in Directory.GetFiles(folder) where Path.GetExtension(file) == ".dll" && files.Contains(Path.GetFileName(file)) select Assembly.LoadFrom(file);
        }

    }
}