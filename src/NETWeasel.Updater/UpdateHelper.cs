using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NETWeasel.Updater
{
    internal static class UpdateHelper
    {
        internal static void StartUpdater(string baseDir, Dictionary<string, string> args)
        {
            var updaterPath = Path.Combine(baseDir, "tools", "NETWeasel.Updater.exe");

            var startupArgument =
                string.Join(" ",
                    args.Select(x => string.IsNullOrWhiteSpace(x.Key) ? $"-{x.Key}" : $"-{x.Key}=\"{x.Value}\""));

            Process.Start(updaterPath, startupArgument);
        }
    }
}