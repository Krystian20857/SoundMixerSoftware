using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG

[assembly: AssemblyConfiguration("DEBUG")]
[assembly: AssemblyDescription("Debug build")]

#else

[assembly: AssemblyConfiguration("RELEASE")]
[assembly: AssemblyDescription("Release build")]

#endif

[assembly: AssemblyVersion("0.0.1")]
[assembly: AssemblyFileVersion("0.0.1")]
[assembly: AssemblyInformationalVersion("0.0.1")]
[assembly: ComVisible(false)]
[assembly: AssemblyProduct("Sound Mixer Software ")]
[assembly: AssemblyCopyright("GPL-3.0 License")]

