using System.Reflection;
// Switch beetwen configurantios.
#if DEBUG
 // Debug only used for development porpuses.
 [assembly: AssemblyConfiguration("Debug")]
#else
 // Only used for production porpuses. 
 [assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyVersion("3.1.2")]
[assembly: AssemblyFileVersion("3.1.2.0")]
