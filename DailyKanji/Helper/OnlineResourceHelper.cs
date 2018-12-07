using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace DailyKanji.Helper
{
    public static class OnlineResourceHelper
    {
        /// <summary>
        /// Return the <see cref="Version"/> from <see cref="AssemblyVersionAttribute"/> of a AssemblyInfo file
        /// </summary>
        /// <param name="linkToAssemlyInfoFile">link to a AssemblyInfo file</param>
        /// <returns>The <see cref="Version"/> from <see cref="AssemblyVersionAttribute"/></returns>
        public static Version GetVersion(string linkToAssemlyInfoFile)
        {
            var webClient             = new WebClient();
            var webData               = webClient.DownloadString(linkToAssemlyInfoFile);
            var webDataSplit          = webData.Split('\n');
            var assemblyVersionLine   = webDataSplit?.FirstOrDefault(found => found?.StartsWith("[assembly: AssemblyVersion") == true);
            var assemblyVersionString = assemblyVersionLine?.Split('"').ElementAtOrDefault(1);

            return new Version(assemblyVersionString);
        }
    }
}
