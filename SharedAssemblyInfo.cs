using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Debug build")]

#else

[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("Release build")]

#endif

[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyFileVersion("0.0.1.0")]
[assembly: AssemblyInformationalVersion("0.0.1.0")]
[assembly: ComVisible(false)]
[assembly: AssemblyProduct("Sound Mixer Software ")]
[assembly: AssemblyCopyright("GPL-3.0 License")]

