using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper class to easier work with <see cref="Assembly"/>s
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// Return the target framework of the assembly of the given class
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the given class</typeparam>
        /// <param name="assemblyClass">A class of the assembly (typical <c>this</c></param>
        /// <returns>A target framework</returns>
        public static string GetTargetFramework<T>(in T assemblyClass) where T : class
            => assemblyClass
                .GetType()
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName
            ?? string.Empty;

        /// <summary>
        /// Return the assembly version of the assembly of the given class
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the given class</typeparam>
        /// <param name="assemblyClass">A class of the assembly (typical <c>this</c></param>
        /// <returns>A assembly version</returns>
        public static Version GetAssemblyVersion<T>(in T assemblyClass) where T : class
            => assemblyClass
                .GetType()
                .GetTypeInfo()
                .Assembly
                .GetName()
                .Version
            ?? new Version();
    }
}
