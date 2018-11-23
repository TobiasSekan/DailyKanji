using System;
using System.Reflection;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper class to eaiser work with <see cref="Assembly"/>s
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// Return the assembly version of the given class of a assembly
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the given class</typeparam>
        /// <param name="assemblyClass">A class of the assembly (typical <c>this</c></param>
        /// <returns>The assembly version</returns>
        public static Version GetAssemblyVersion<T>(T assemblyClass) where T : class
            => assemblyClass?.GetType().GetTypeInfo().Assembly?.GetName().Version;
    }
}
