using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NETWeasel.Common;

namespace NETWeasel.Updater
{
    internal class Bootstrapper
    {
        private string _installPath;

        internal void Run(IEnumerable<string> args)
        {
            var options = new OptionSet
            {
                {"path=", "Install path of application being bootstrapped", param => _installPath = param },
            };

            options.Parse(args);

            var parentDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            FileHelper.CopyDirectory(parentDir.FullName, _installPath);
        }
    }
}
