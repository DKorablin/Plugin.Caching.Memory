using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("09b87c30-0514-465b-81c7-0bff66dad84b")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://github.com/DKorablin/Plugin.Caching.Memory")]
#else

[assembly: AssemblyTitle("Plugin.Caching.Memory")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.Caching.Memory")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2016")]
#endif