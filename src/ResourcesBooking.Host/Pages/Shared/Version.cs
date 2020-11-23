using System.IO;
using System.Linq;
using System.Reflection;

namespace ResourcesBooking.Host.Pages
{
    public static class Version 
    {
        private static string _fullVersion = GetVersion();
        public static string FullVersion => _fullVersion;

        private static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var versionResourceName = assembly.GetManifestResourceNames().First(it => it.Contains("VERSION"));

            using(var stream = assembly.GetManifestResourceStream(versionResourceName))
            using(var streamReader = new StreamReader(stream))
            {
                return $"v{streamReader.ReadToEnd()}";
            }
        }
    }
}
