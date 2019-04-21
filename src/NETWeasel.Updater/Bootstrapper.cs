using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NETWeasel.Common;

namespace NETWeasel.Updater
{
    internal class Bootstrapper
    {
        private Mode _mode;
        private int _processId;
        private string _installPath;
        private bool _restartApp;
        private string _cleanupPath;

        internal void Run(IEnumerable<string> args)
        {
            var options = new OptionSet
            {
                {"mode=", "Mode for bootstrapper (Update, Cleanup)", param => _mode = (Mode)Enum.Parse(typeof(Mode), param, true) },
                {"proc=", "Process name to shut down", param => int.TryParse(param, out _processId) },
                {"path=", "Install path of application being bootstrapped", param => _installPath = param },
                {"cleanupPath=", "Path to cleanup in cleanup mode", param => _cleanupPath = param },
                {"restartApp", "Switch to automatically start application after bootstrapper is done", param => _restartApp = true },
            };

            options.Parse(args);

            switch (_mode)
            {
                case Mode.Update:
                    DoUpdate();
                    break;
                case Mode.Cleanup:
                    DoCleanup();
                    break;
                default:
                    return;
            }
        }

        private void DoUpdate()
        {
            var process = Process.GetProcessById(_processId);

            process.Kill();

            var parentDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            FileHelper.CopyDirectory(parentDir.FullName, _installPath);

            UpdateHelper.StartUpdater(_installPath, new Dictionary<string, string>
            {
                ["mode"] = "cleanup",
                ["cleanupPath"] = _cleanupPath,
            });
        }

        private void DoCleanup()
        {
            Directory.Delete(_cleanupPath, true);
        }
    }
}
