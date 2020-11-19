using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace DailyKanji.Helper
{
    /// <summary>
    /// Helper class to easier work with online resources
    /// </summary>
    internal static class OnlineResourceHelper
    {
        /// <summary>
        /// Return the <see cref="Version"/> from <see cref="AssemblyVersionAttribute"/> of a AssemblyInfo file
        /// </summary>
        /// <param name="linkToAssemlyInfoFile">link to a AssemblyInfo file</param>
        /// <returns>The <see cref="Version"/> from <see cref="AssemblyVersionAttribute"/></returns>
        internal static Version GetVersion(string linkToAssemlyInfoFile)
        {
            string webData;

            using(var webClient = new WebClient())
            {
                webData = webClient.DownloadString(linkToAssemlyInfoFile);
            }

            var webDataSplit          = webData.Split('\n');
            var assemblyVersionLine   = Array.Find(webDataSplit,
                                                   found => found?.StartsWith("[assembly: AssemblyVersion", StringComparison.OrdinalIgnoreCase) == true);

            var assemblyVersionString = assemblyVersionLine?.Split('"').ElementAtOrDefault(1);

            return string.IsNullOrWhiteSpace(assemblyVersionString)
                    ? new Version()
                    : new Version(assemblyVersionString);
        }
    }
}
