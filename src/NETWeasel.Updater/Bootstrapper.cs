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
        private bool _verbose;

        internal void Run(IEnumerable<string> args)
        {
            Console.WriteLine("Starting Bootstrapper...");

            var options = new OptionSet
            {
                {"mode=", "Mode for bootstrapper (Update, Cleanup)", param => _mode = (Mode)Enum.Parse(typeof(Mode), param, true) },
                {"proc=", "Process name to shut down", param => int.TryParse(param, out _processId) },
                {"path=", "Install path of application being bootstrapped", param => _installPath = param },
                {"cleanupPath=", "Path to cleanup in cleanup mode", param => _cleanupPath = param },
                {"restartApp", "Switch to automatically start application after bootstrapper is done", param => _restartApp = true },
                {"verbose", param => _verbose = true },
            };

            options.Parse(args);

            Log("Args parsed");

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
            Log("Starting update...");

            Log($"Looking for process {_processId}");

            var process = Process.GetProcessById(_processId);

            Log("Killing process...");

            process.Kill();

            Log("Process killed");

            var originDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var targetDir = Path.GetDirectoryName(_installPath);

            Log($"Installing update from origin: {originDir} to target: {targetDir}");

            FileHelper.CopyDirectory(originDir, targetDir);

            Log("Update installed");

            Log("Spawning cleanup bootstrapper");

            UpdateHelper.StartUpdater(_installPath, new Dictionary<string, string>
            {
                ["mode"] = "cleanup",
                ["cleanupPath"] = _cleanupPath,
            });

            if (_restartApp)
            {
                Log("App was requested to restart, spawning updated app");
                Process.Start(_installPath);
                Log("App spawned");
            }

            Log("Done updating");
        }

        private void DoCleanup()
        {
            Log($"Starting cleanup, removing dir {_cleanupPath}");

            try
            {
                Directory.Delete(_cleanupPath, true);

                Log("Finished cleanup");
            }
            catch (Exception ex)
            {
                Log($"Failed cleanup with message: {ex.Message}, ignoring and continuing");
            }
        }

        private void Log(string message)
        {
            if (!_verbose)
                return;

            Console.WriteLine(message);
        }
    }
}
