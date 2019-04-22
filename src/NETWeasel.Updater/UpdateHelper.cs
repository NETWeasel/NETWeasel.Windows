using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NETWeasel.Updater
{
    internal static class UpdateHelper
    {
        internal static void StartUpdater(string baseDir, Dictionary<string, string> args, bool elevateProcess)
        {
            var updaterPath = Path.Combine(baseDir, "NETWeasel.Updater.FX.exe");

            var startupArgument =
                string.Join(" ",
                    args.Select(x => string.IsNullOrWhiteSpace(x.Key) ? $"-{x.Key}" : $"-{x.Key}=\"{x.Value}\""));

            var procInfo = new ProcessStartInfo(updaterPath)
            {
                Arguments = startupArgument
            };

            if (elevateProcess)
            {
                procInfo.Verb = "runas";
            }

            Process.Start(procInfo);
        }
    }
}